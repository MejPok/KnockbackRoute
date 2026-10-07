using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Builds a linear run by joining each room's Entry connector to the previous
/// room's Exit connector. Connector directions are cardinal relative to the prefab.
/// </summary>
public class RoomGenerator : MonoBehaviour
{
    [Header("Room pool")]
    [SerializeField] private RoomDefinition[] rooms;
    [SerializeField, Min(1)] private int roomCount = 8;
    [SerializeField] private Transform generatedRoomsRoot;
    [SerializeField, Min(1)] private int maxGenerationAttempts = 2000;

    [Header("Seed")]
    [SerializeField] private int seed;
    [SerializeField] private bool createSeedOnStart = true;
    [SerializeField] private bool generateOnStart = true;

    public int CurrentSeed { get; private set; }

    private void Start()
    {
        if (generateOnStart)
            Generate();
    }

    /// <summary>Generates a run using a fresh seed or the configured seed.</summary>
    public void Generate()
    {
        int runSeed = createSeedOnStart ? Environment.TickCount : seed;
        GenerateWithSeed(runSeed);
    }

    /// <summary>Generates a run from an explicit seed for replayable runs.</summary>
    public void GenerateWithSeed(int runSeed)
    {
        if (rooms == null || rooms.Length == 0)
        {
            Debug.LogError("RoomGenerator needs at least one RoomDefinition.", this);
            return;
        }

        //set a random seed
        CurrentSeed = runSeed;
        seed = CurrentSeed;
        var random = new System.Random(CurrentSeed);


        //jestli neni root kam davat mistnosti da to sem
        Transform root = GetGeneratedRoomsRoot();
        ClearGeneratedRooms(root);

        //jestli se daji mistnosti pouzit
        List<RoomDefinition> starts = GetRoomCandidates(null);
        if (starts.Count == 0)
        {
            Debug.LogError("No valid RoomDefinition is available.", this);
            return;
        }



        int remainingAttempts = maxGenerationAttempts;
        while (starts.Count > 0 && remainingAttempts > 0)
        {
            int startIndex = random.Next(starts.Count);
            RoomDefinition firstDefinition = starts[startIndex];
            starts.RemoveAt(startIndex);

            GameObject firstRoom = Instantiate(firstDefinition.prefab, root);
            firstRoom.transform.localPosition = Vector3.zero;
            firstRoom.transform.localRotation = Quaternion.identity;

            RoomConnector firstExit = FindConnector(firstRoom, RoomConnector.ConnectionType.Exit); //najdne exity
            BoxCollider2D[] firstFootprint = FindFootprint(firstRoom); //najde collider

            //collider check
            if (firstFootprint == null || (roomCount > 1 && firstExit == null))
            {
                Debug.LogWarning($"Skipping start room '{firstDefinition.name}': it needs a root BoxCollider2D footprint and an Exit connector.", firstRoom);
                Destroy(firstRoom);
                continue;
            }

            var placedFootprints = new List<BoxCollider2D>();
            foreach (var footprint in firstFootprint) {
                placedFootprints.Add(footprint);
            }


            if (roomCount == 1 || TryGenerateNextRoom(1, firstExit, firstRoom.transform, root, placedFootprints, random, ref remainingAttempts))
            {
                Debug.Log($"Generated {roomCount} rooms with seed {CurrentSeed}.", this);
                return;
            }

            Destroy(firstRoom);
            Physics2D.SyncTransforms();
        }

        Debug.LogWarning($"Could not generate {roomCount} non-overlapping rooms with seed {CurrentSeed} after {maxGenerationAttempts} placement attempts. Try another seed, more room layouts, or a higher attempt limit.", this);
    }

    private bool TryGenerateNextRoom(
        int roomIndex,
        RoomConnector previousExit,
        Transform previousRoomTransform,
        Transform root,
        List<BoxCollider2D> placedFootprints,
        System.Random random,
        ref int remainingAttempts)
    {
        if (roomIndex >= roomCount)
            return true;

        List<RoomDefinition> candidates = GetRoomCandidates(RoomConnector.ConnectionType.Entry); //najde vstupy

        while (candidates.Count > 0 && remainingAttempts > 0)
        {
            remainingAttempts--;
            int candidateIndex = random.Next(candidates.Count); //vybere dalsi mistnost

            RoomDefinition definition = candidates[candidateIndex];
            candidates.RemoveAt(candidateIndex);

            GameObject room = Instantiate(definition.prefab, root);

            RoomConnector entry = FindConnector(room, RoomConnector.ConnectionType.Entry);
            RoomConnector exit = FindConnector(room, RoomConnector.ConnectionType.Exit);

            BoxCollider2D[] footprint = FindFootprint(room);

            if (entry == null || exit == null || footprint == null)
            {
                Debug.LogWarning($"Skipping room '{definition.name}': it needs Entry and Exit connectors plus a root BoxCollider2D footprint.", room);
                Destroy(room);
                continue;
            }

            Vector2 entryFacing = GetWorldFacing(entry, room.transform);
            Vector2 exitFacing = GetWorldFacing(previousExit, previousRoomTransform);
            float angle = Vector2.SignedAngle(entryFacing, -exitFacing);
            room.transform.Rotate(0f, 0f, angle, Space.World);


            Vector3 offset = previousExit.transform.position - entry.transform.position;
            room.transform.position += offset; //nastavi na entry/exit

            Physics2D.SyncTransforms();

            bool overlaps = false;
            foreach (BoxCollider2D box in footprint)
            {
                if (box != null && OverlapsPlacedRoom(box, placedFootprints))
                {
                    overlaps = true;
                    break;
                }
            }

            if (overlaps)
            {
                Destroy(room);
                Physics2D.SyncTransforms();
                continue;
            }

            int addedFootprintCount = footprint.Length;
            placedFootprints.AddRange(footprint);

            if (TryGenerateNextRoom(
                roomIndex + 1,
                exit,
                room.transform,
                root,
                placedFootprints,
                random,
                ref remainingAttempts))
            {
                return true;
            }

            // Backtrack: remove ALL colliders belonging to this room
            placedFootprints.RemoveRange(
                placedFootprints.Count - addedFootprintCount,
                addedFootprintCount
            );

            Destroy(room);
            Physics2D.SyncTransforms();
        }

        return false;
    }
    private List<RoomDefinition> GetRoomCandidates(RoomConnector.ConnectionType? requiredType)
    {
        var candidates = new List<RoomDefinition>();
        foreach (RoomDefinition definition in rooms)
        {
            if (definition == null || definition.prefab == null)
                continue;

            if (requiredType.HasValue && FindConnector(definition.prefab, requiredType.Value) == null)
                continue;

            // Every room used after the first must also provide an Exit for chaining.
            if (requiredType == RoomConnector.ConnectionType.Entry &&
                FindConnector(definition.prefab, RoomConnector.ConnectionType.Exit) == null)
                continue;

            candidates.Add(definition);
        }

        return candidates;
    }

    private static BoxCollider2D[] FindFootprint(GameObject room)
    {
        // Keep this collider dedicated to the room footprint, separate from gameplay colliders.
        return room.GetComponent<RoomFootprint>().boxColliders;
    }

    private static bool OverlapsPlacedRoom(BoxCollider2D candidate, List<BoxCollider2D> placedFootprints)
    {
        foreach (BoxCollider2D placed in placedFootprints)
        {
            if (placed == null)
                continue;

            // Collider distance uses actual rotated box shapes, avoiding AABB false positives.
            if (Physics2D.Distance(candidate, placed).isOverlapped)
                return true;
        }

        return false;
    }

    private static RoomConnector FindConnector(GameObject room, RoomConnector.ConnectionType type)
    {
        RoomConnector[] connectors = room.GetComponentsInChildren<RoomConnector>(true);
        foreach (RoomConnector connector in connectors)
        {
            if (connector.type == type)
                return connector;
        }

        return null;
    }

    private Transform GetGeneratedRoomsRoot()
    {
        if (generatedRoomsRoot != null)
            return generatedRoomsRoot;

        var rootObject = new GameObject("Generated Rooms");
        generatedRoomsRoot = rootObject.transform;
        generatedRoomsRoot.SetParent(transform, false);
        return generatedRoomsRoot;
    }

    private static Vector2 GetWorldFacing(RoomConnector connector, Transform roomTransform)
    {
        Vector2 localFacing;
        switch (connector.direction)
        {
            case RoomConnector.Direction.North: localFacing = Vector2.up; break;
            case RoomConnector.Direction.East: localFacing = Vector2.right; break;
            case RoomConnector.Direction.South: localFacing = Vector2.down; break;
            case RoomConnector.Direction.West: localFacing = Vector2.left; break;
            default: localFacing = Vector2.up; break;
        }

        return roomTransform.TransformDirection(localFacing);
    }

    private static void ClearGeneratedRooms(Transform root)
    {
        // Only remove children owned by this generator's output root.
        for (int i = root.childCount - 1; i >= 0; i--)
            Destroy(root.GetChild(i).gameObject);
    }
}
