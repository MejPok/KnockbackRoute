using UnityEngine;
using System.Collections.Generic;

public class RoomPlacer : MonoBehaviour
{
    Transform roomRoot;
    private readonly List<BoxCollider2D> placedFootprints = new List<BoxCollider2D>();
    private RoomConnector lastExit;
    private Transform lastRoom;

    public bool GenerateRoomsWithBacktracking(RoomDefinition[] candidates, Transform root, int count)
    {
        roomRoot = root;
        if (count <= 0) return true;
        if (lastExit == null || lastRoom == null || candidates == null) return false;

        return TryGenerateRooms(candidates, count);
    }

    private bool TryGenerateRooms(RoomDefinition[] candidates, int remaining)
    {
        if (remaining == 0) return true;

        // Randomize each choice order, then undo placements when a later room cannot fit.
        var shuffled = new List<RoomDefinition>(candidates);
        for (int i = shuffled.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (shuffled[i], shuffled[j]) = (shuffled[j], shuffled[i]);
        }

        foreach (RoomDefinition definition in shuffled)
        {
            if (definition == null || definition.prefab == null) continue;
            GameObject room = Instantiate(definition.prefab, roomRoot);
            RoomConnector entry = FindConnector(room, RoomConnector.ConnectionType.Entry);
            RoomConnector exit = FindConnector(room, RoomConnector.ConnectionType.Exit);
            BoxCollider2D[] footprints = FindFootprints(room);
            if (entry == null || exit == null || footprints == null || footprints.Length == 0)
            {
                DestroyRoom(room);
                continue;
            }

            AlignRoom(room.transform, entry, lastExit, lastRoom);
            Physics2D.SyncTransforms();
            if (OverlapsPlacedRoom(footprints))
            {
                DestroyRoom(room);
                Physics2D.SyncTransforms();
                continue;
            }

            RoomConnector previousExit = lastExit;
            Transform previousRoom = lastRoom;
            placedFootprints.AddRange(footprints);
            lastExit = exit;
            lastRoom = room.transform;
            if (TryGenerateRooms(candidates, remaining - 1)) return true;

            lastExit = previousExit;
            lastRoom = previousRoom;
            foreach (BoxCollider2D footprint in footprints) placedFootprints.Remove(footprint);
            DestroyRoom(room);
            Physics2D.SyncTransforms();
        }

        return false;
    }

    public void PlaceFirstRoom(RoomDefinition roomToPlace, Transform root)
    {
        roomRoot = root;

        var firstRoom = Instantiate(roomToPlace.prefab, roomRoot);
        firstRoom.transform.localPosition = Vector3.zero;
        firstRoom.transform.localRotation = Quaternion.identity;

        CorrectLastRoomPlacement(firstRoom);
    }

    void CorrectLastRoomPlacement(GameObject lastGeneratedRoom)
    {
        placedFootprints.AddRange(FindFootprints(lastGeneratedRoom));
        lastRoom = lastGeneratedRoom.transform;
        lastExit = FindConnector(lastGeneratedRoom, RoomConnector.ConnectionType.Exit);
    }

    
    private void ClearGeneratedRooms()
    {
        for (int i = roomRoot.childCount - 1; i >= 0; i--)
            DestroyRoom(roomRoot.GetChild(i).gameObject);
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
