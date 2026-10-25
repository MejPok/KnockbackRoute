using System.Collections.Generic;
using UnityEngine;

public class RoomPlacer : MonoBehaviour
{
    private const int LookaheadRoomCount = 2;

    private sealed class CandidateFrame
    {
        public readonly List<RoomDefinition> candidates;
        public int nextCandidate;

        public CandidateFrame(RoomDefinition[] source)
        {
            candidates = new List<RoomDefinition>(source);
        }
    }

    private Transform roomRoot;
    private readonly List<BoxCollider2D> placedFootprints = new List<BoxCollider2D>();
    // Retained per depth so later calls can revisit choices from earlier sections.
    private readonly List<CandidateFrame> generationFrames = new List<CandidateFrame>();
    private CandidateFrame startFrame;
    public RoomConnector lastExit;
    private Transform lastRoom;

    public int RoomsPlaced => roomRoot != null ? roomRoot.childCount : 0;

    public bool GenerateRoomsUpToCount(RoomDefinition[] candidates, Transform root, int targetRoomCount,
        int maxAttempts, int minimumRoomCountToKeep)
    {
        roomRoot = root;
        if (roomRoot == null || targetRoomCount <= 0) return false;
        minimumRoomCountToKeep = Mathf.Clamp(minimumRoomCountToKeep, 1, roomRoot.childCount);
        if (roomRoot.childCount >= targetRoomCount) return true;

        if (!CanGenerate(candidates)) return false;

        int attemptsRemaining = Mathf.Max(1, maxAttempts);
        while (HasMoreRoomsToPlace(targetRoomCount) && attemptsRemaining > 0)
        {
            if (TryPlaceAtCurrentDepth(candidates, targetRoomCount, ref attemptsRemaining)) continue;
            if (attemptsRemaining == 0 || !BacktrackFromCurrentDepth(minimumRoomCountToKeep)) break;
        }

        return HasReachedTarget(targetRoomCount);
    }
    private bool CanGenerate(RoomDefinition[] candidates)
    {
        return lastExit != null && lastRoom != null && candidates != null && candidates.Length > 0;
    }

    private bool HasMoreRoomsToPlace(int targetRoomCount)
    {
        return roomRoot.childCount < targetRoomCount;
    }

    private bool HasReachedTarget(int targetRoomCount)
    {
        return roomRoot.childCount >= targetRoomCount;
    }

    private bool TryPlaceAtCurrentDepth(RoomDefinition[] candidates, int targetRoomCount, ref int attemptsRemaining)
    {
        CandidateFrame frame = GetCurrentFrame(candidates);
        return TryPlaceNextCandidate(frame, targetRoomCount, candidates, ref attemptsRemaining);
    }

    private CandidateFrame GetCurrentFrame(RoomDefinition[] candidates)
    {
        int depth = roomRoot.childCount - 1;
        if (generationFrames.Count == depth)
            generationFrames.Add(new CandidateFrame(candidates));
        return generationFrames[depth];
    }

    private bool TryPlaceNextCandidate(CandidateFrame frame, int targetRoomCount, RoomDefinition[] candidates,
        ref int attemptsRemaining)
    {
        while (frame.nextCandidate < frame.candidates.Count && attemptsRemaining > 0)
        {
            RoomDefinition candidate = frame.candidates[frame.nextCandidate++];
            attemptsRemaining--;
            if (!TryPlaceRoom(candidate)) continue;

            int roomsToCheck = Mathf.Min(LookaheadRoomCount, targetRoomCount - roomRoot.childCount);
            if (HasValidContinuation(candidates, roomsToCheck, ref attemptsRemaining)) return true;

            RemoveLastRoom();
        }
        return false;
    }

    private bool HasValidContinuation(RoomDefinition[] candidates, int roomsToCheck, ref int attemptsRemaining)
    {
        if (roomsToCheck <= 0) return true;

        var Candidates = new List<RoomDefinition>(candidates);
        foreach (RoomDefinition candidate in Candidates)
        {
            if (attemptsRemaining <= 0) return false;
            attemptsRemaining--;
            if (!TryPlaceRoom(candidate)) continue;

            bool canContinue = HasValidContinuation(candidates, roomsToCheck - 1, ref attemptsRemaining);
            RemoveLastRoom();
            if (canContinue) return true;
        }

        return false;
    }

    

    private bool BacktrackFromCurrentDepth(int minimumRoomCountToKeep)
    {
        if (roomRoot.childCount <= minimumRoomCountToKeep)
            return false;

        int depth = roomRoot.childCount - 1;
        RemoveExhaustedFrame(depth);
        RemoveLastRoom();
        return true;
    }
    private bool TryNextStartRoom()
    {
        if (roomRoot.childCount == 0) return false;

        GameObject currentStart = roomRoot.GetChild(0).gameObject;
        RemoveRoomFootprints(currentStart);
        DestroyRoom(currentStart);
        lastExit = null;
        lastRoom = null;
        Physics2D.SyncTransforms();
        generationFrames.Clear();
        return TryPlaceStartCandidate();
    }

    private void RemoveExhaustedFrame(int depth)
    {
        if (generationFrames.Count > depth)
            generationFrames.RemoveAt(depth);
    }

    private bool TryPlaceRoom(RoomDefinition definition)
    {
        if (definition == null || definition.prefab == null) return false;

        GameObject room = Instantiate(definition.prefab, roomRoot);
        if (!TryGetPlacementParts(room, out RoomConnector entry, out RoomConnector exit, out BoxCollider2D[] footprints))
        {
            DestroyRoom(room);
            return false;
        }

        AlignRoom(room.transform, entry, lastExit, lastRoom);
        Physics2D.SyncTransforms();
        if (OverlapsPlacedRoom(footprints))
        {
            DestroyRoom(room);
            Physics2D.SyncTransforms();
            return false;
        }

        RegisterPlacedRoom(room, exit, footprints);
        return true;
    }

    private bool TryGetPlacementParts(GameObject room, out RoomConnector entry, out RoomConnector exit,
        out BoxCollider2D[] footprints)
    {
        entry = FindConnector(room, RoomConnector.ConnectionType.Entry);
        exit = FindConnector(room, RoomConnector.ConnectionType.Exit);
        footprints = FindFootprints(room);
        return entry != null && exit != null && footprints != null && footprints.Length > 0;
    }

    private void RegisterPlacedRoom(GameObject room, RoomConnector exit, BoxCollider2D[] footprints)
    {
        placedFootprints.AddRange(footprints);
        lastExit = exit;
        lastRoom = room.transform;
    }

    private void RemoveLastRoom()
    {
        GameObject room = roomRoot.GetChild(roomRoot.childCount - 1).gameObject;
        RemoveRoomFootprints(room);
        DestroyRoom(room);
        RestoreLastConnector();
        Physics2D.SyncTransforms();
    }

    public int RollbackLastRooms(int roomCount)
    {
        if (roomRoot == null || roomRoot.childCount <= 1) return 0;

        int roomsToRemove = Mathf.Min(Mathf.Max(0, roomCount), roomRoot.childCount - 1);
        for (int i = 0; i < roomsToRemove; i++)
            RemoveLastRoom();

        KeepChoicesAtCurrentEnd();
        return roomsToRemove;
    }

    private void KeepChoicesAtCurrentEnd()
    {
        int nextRoomDepth = roomRoot.childCount - 1;
        while (generationFrames.Count > nextRoomDepth + 1)
            generationFrames.RemoveAt(generationFrames.Count - 1);
    }

    private void RemoveRoomFootprints(GameObject room)
    {
        BoxCollider2D[] footprints = FindFootprints(room);
        if (footprints == null) return;
        foreach (BoxCollider2D footprint in footprints)
            placedFootprints.Remove(footprint);
    }

    private void RestoreLastConnector()
    {
        lastRoom = roomRoot.GetChild(roomRoot.childCount - 1);
        lastExit = FindConnector(lastRoom.gameObject, RoomConnector.ConnectionType.Exit);
    }

    public bool PlaceFirstRoom(RoomDefinition[] candidates, Transform root)
    {
        roomRoot = root;
        ResetPlacementState();
        startFrame = new CandidateFrame(candidates ?? System.Array.Empty<RoomDefinition>());
        return TryPlaceStartCandidate();
    }

    private bool TryPlaceStartCandidate()
    {
        while (startFrame != null && startFrame.nextCandidate < startFrame.candidates.Count)
        {
            RoomDefinition definition = startFrame.candidates[startFrame.nextCandidate++];
            if (definition == null || definition.prefab == null) continue;

            GameObject firstRoom = Instantiate(definition.prefab, roomRoot);
            RoomConnector exit = FindConnector(firstRoom, RoomConnector.ConnectionType.Exit);
            BoxCollider2D[] footprints = FindFootprints(firstRoom);
            if (exit == null || footprints == null || footprints.Length == 0)
            {
                DestroyRoom(firstRoom);
                continue;
            }

            firstRoom.transform.localPosition = Vector3.zero;
            firstRoom.transform.localRotation = Quaternion.identity;
            RegisterPlacedRoom(firstRoom, exit, footprints);
            return true;
        }

        return false;
    }

    private void ResetPlacementState()
    {
        generationFrames.Clear();
        startFrame = null;
        placedFootprints.Clear();
        lastExit = null;
        lastRoom = null;
    }

    private static void DestroyRoom(GameObject room)
    {
        room.transform.SetParent(null);
        Object.Destroy(room);
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

    private bool OverlapsPlacedRoom(BoxCollider2D[] candidates)
    {
        foreach (BoxCollider2D candidate in candidates)
            foreach (BoxCollider2D existing in placedFootprints)
                if (candidate != null && existing != null && Physics2D.Distance(candidate, existing).isOverlapped)
                    return true;
        return false;
    }
}
