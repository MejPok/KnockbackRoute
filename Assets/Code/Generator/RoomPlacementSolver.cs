using System.Collections.Generic;
using UnityEngine;

/// <summary>Places room prefabs, aligns connectors, checks overlap, and backtracks on failure.</summary>
internal sealed class RoomPlacementSolver
{
    private readonly Transform root;
    private readonly RoomSelectionPolicy selection;
    private readonly RoomSignalPairing signals;
    private readonly System.Random random;
    private readonly List<BoxCollider2D> placedFootprints = new List<BoxCollider2D>();

    private RoomConnector lastExit;
    private Transform lastRoom;

    public int RoomsGenerated { get; private set; }

    public RoomPlacementSolver(Transform root, RoomSelectionPolicy selection, RoomSignalPairing signals, System.Random random)
    {
        this.root = root;
        this.selection = selection;
        this.signals = signals;
        this.random = random;
    }

    public void Reset()
    {
        signals.Reset();
        ClearGeneratedRooms();
        Physics2D.SyncTransforms();
        placedFootprints.Clear();
        RoomsGenerated = 0;
        lastExit = null;
        lastRoom = null;
    }

    public bool GenerateSection(int totalRoomCount, int sectionSize, int maxAttempts,
        out int roomsAdded, out string failureReason)
    {
        roomsAdded = 0;
        failureReason = null;
        if (RoomsGenerated >= totalRoomCount) return true;

        int targetCount = Mathf.Min(Mathf.Max(1, sectionSize), totalRoomCount - RoomsGenerated);
        if (RoomsGenerated == 0)
            return TryGenerateInitialSection(targetCount, totalRoomCount, maxAttempts, out roomsAdded, out failureReason);

        return TryGenerateNextSection(targetCount, maxAttempts, out roomsAdded, out failureReason);
    }

    private bool TryGenerateInitialSection(int targetCount, int totalRoomCount, int maxAttempts,
        out int roomsAdded, out string failureReason)
    {
        roomsAdded = 0;
        failureReason = null;
        List<RoomDefinition> starts = selection.GetCandidates(null, 0);
        starts.RemoveAll(room => !signals.CanPlaceDoors(room.prefab, signals.BuildAvailability()));
        Shuffle(starts);
        if (starts.Count == 0)
        {
            failureReason = "The contract has no eligible starting room.";
            return false;
        }

        int remainingAttempts = maxAttempts;
        foreach (RoomDefinition definition in starts)
        {
            if (remainingAttempts-- <= 0) break;
            GameObject start = Object.Instantiate(definition.prefab, root);
            start.transform.localPosition = Vector3.zero;
            start.transform.localRotation = Quaternion.identity;
            RoomConnector exit = FindConnector(start, RoomConnector.ConnectionType.Exit);
            BoxCollider2D[] footprint = FindFootprints(start);
            if ((totalRoomCount > 1 && exit == null) || footprint == null || footprint.Length == 0)
            {
                Debug.LogWarning($"Start room '{definition.name}' needs an Exit connector and RoomFootprint colliders.", start);
                DestroyRoom(start);
                continue;
            }

            placedFootprints.AddRange(footprint);
            signals.CommitRoom(start);
            RoomsGenerated = 1;
            lastExit = exit;
            lastRoom = start.transform;
            int appendedCount = 0;
            int remainingTarget = targetCount - 1;
            RoomConnector sectionExit = lastExit;
            Transform sectionEnd = lastRoom;
            bool sectionCompleted = remainingTarget == 0 || TryAppendRooms(RoomsGenerated, remainingTarget,
                ref appendedCount, ref remainingAttempts, lastExit, lastRoom,
                signals.BuildAvailability(), out sectionExit, out sectionEnd);
            if (sectionCompleted)
            {
                if (remainingTarget > 0)
                {
                    for (int i = 1; i < root.childCount; i++)
                        signals.CommitRoom(root.GetChild(i).gameObject);
                    lastExit = sectionExit;
                    lastRoom = sectionEnd;
                }
                RoomsGenerated += appendedCount;
                roomsAdded = 1 + appendedCount;
                return true;
            }

            Reset();
        }

        failureReason = $"Could not place the first section within {maxAttempts} attempts. Check contract steps and room connectors.";
        return false;
    }

    private bool TryGenerateNextSection(int targetCount, int maxAttempts,
        out int roomsAdded, out string failureReason)
    {
        roomsAdded = 0;
        failureReason = null;
        int addedCount = 0;
        int remainingAttempts = maxAttempts;
        if (!TryAppendRooms(RoomsGenerated, targetCount, ref addedCount, ref remainingAttempts,
                lastExit, lastRoom, signals.BuildAvailability(), out RoomConnector sectionExit, out Transform sectionEnd))
        {
            failureReason = $"Could not place the next section within {maxAttempts} attempts. Check contract steps and room connectors.";
            return false;
        }

        int firstNewChildIndex = root.childCount - addedCount;
        for (int i = firstNewChildIndex; i < root.childCount; i++)
            signals.CommitRoom(root.GetChild(i).gameObject);

        RoomsGenerated += addedCount;
        roomsAdded = addedCount;
        lastExit = sectionExit;
        lastRoom = sectionEnd;
        return true;
    }

    private bool TryAppendRooms(int nextRoomIndex, int targetCount, ref int addedCount, ref int remainingAttempts,
        RoomConnector previousExit, Transform previousRoom, Dictionary<string, int> availableSignals,
        out RoomConnector resultExit, out Transform resultRoom)
    {
        resultExit = previousExit;
        resultRoom = previousRoom;
        if (addedCount >= targetCount) return true;

        List<RoomDefinition> candidates = selection.GetCandidates(RoomConnector.ConnectionType.Entry, nextRoomIndex);
        Shuffle(candidates);
        foreach (RoomDefinition definition in candidates)
        {
            if (!signals.CanPlaceDoors(definition.prefab, availableSignals)) continue;
            if (remainingAttempts-- <= 0) return false;

            GameObject room = Object.Instantiate(definition.prefab, root);
            RoomConnector entry = FindConnector(room, RoomConnector.ConnectionType.Entry);
            RoomConnector exit = FindConnector(room, RoomConnector.ConnectionType.Exit);
            BoxCollider2D[] footprint = FindFootprints(room);
            if (entry == null || exit == null || footprint == null || footprint.Length == 0)
            {
                Debug.LogWarning($"Room '{definition.name}' needs Entry and Exit connectors plus RoomFootprint colliders.", room);
                DestroyRoom(room);
                continue;
            }

            AlignRoom(room.transform, entry, previousExit, previousRoom);
            Physics2D.SyncTransforms();
            if (OverlapsPlacedRoom(footprint))
            {
                DestroyRoom(room);
                Physics2D.SyncTransforms();
                continue;
            }

            placedFootprints.AddRange(footprint);
            addedCount++;
            var nextAvailability = new Dictionary<string, int>(availableSignals, System.StringComparer.Ordinal);
            signals.AddRoomToAvailability(room, nextAvailability);
            if (TryAppendRooms(nextRoomIndex + 1, targetCount, ref addedCount, ref remainingAttempts,
                    exit, room.transform, nextAvailability, out resultExit, out resultRoom))
                return true;

            addedCount--;
            RemoveFootprints(footprint);
            DestroyRoom(room);
            Physics2D.SyncTransforms();
        }
        return false;
    }

    private RoomDefinition ChooseCandidate(List<RoomDefinition> candidates) => candidates[random.Next(candidates.Count)];

    private void Shuffle(List<RoomDefinition> candidates)
    {
        for (int i = candidates.Count - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            (candidates[i], candidates[j]) = (candidates[j], candidates[i]);
        }
    }

    private bool OverlapsPlacedRoom(BoxCollider2D[] candidates)
    {
        foreach (BoxCollider2D candidate in candidates)
            foreach (BoxCollider2D existing in placedFootprints)
                if (candidate != null && existing != null && Physics2D.Distance(candidate, existing).isOverlapped)
                    return true;
        return false;
    }

    private void RemoveFootprints(BoxCollider2D[] footprints)
    {
        foreach (BoxCollider2D footprint in footprints) placedFootprints.Remove(footprint);
    }

    private static BoxCollider2D[] FindFootprints(GameObject room)
    {
        RoomFootprint footprint = room.GetComponent<RoomFootprint>();
        return footprint != null ? footprint.boxColliders : null;
    }

    private static RoomConnector FindConnector(GameObject room, RoomConnector.ConnectionType type)
    {
        foreach (RoomConnector connector in room.GetComponentsInChildren<RoomConnector>(true))
            if (connector.type == type) return connector;
        return null;
    }

    private static void AlignRoom(Transform room, RoomConnector entry, RoomConnector previousExit, Transform previousRoom)
    {
        Vector2 entryFacing = GetWorldFacing(entry, room);
        Vector2 exitFacing = GetWorldFacing(previousExit, previousRoom);
        room.Rotate(0f, 0f, Vector2.SignedAngle(entryFacing, -exitFacing), Space.World);
        room.position += previousExit.transform.position - entry.transform.position;
    }

    private static Vector2 GetWorldFacing(RoomConnector connector, Transform roomTransform)
    {
        Vector2 localFacing = connector.direction switch
        {
            RoomConnector.Direction.North => Vector2.up,
            RoomConnector.Direction.East => Vector2.right,
            RoomConnector.Direction.South => Vector2.down,
            RoomConnector.Direction.West => Vector2.left,
            _ => Vector2.up
        };
        return roomTransform.TransformDirection(localFacing);
    }

    private static void DestroyRoom(GameObject room)
    {
        room.transform.SetParent(null);
        Object.Destroy(room);
    }

    private void ClearGeneratedRooms()
    {
        for (int i = root.childCount - 1; i >= 0; i--)
            DestroyRoom(root.GetChild(i).gameObject);
    }
}
