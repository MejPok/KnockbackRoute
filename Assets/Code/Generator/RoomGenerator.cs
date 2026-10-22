using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>Unity entry point for starting and incrementally extending a generated dungeon.</summary>
[RequireComponent(typeof(RoomDecider))]
[RequireComponent(typeof(RoomPlacer))]

public class RoomGenerator : MonoBehaviour
{
    public static RoomGenerator Instance;
    [SerializeField, Min(1)] private int roomCount = 40;
    [SerializeField] private int roomsGenerated;
    [SerializeField] public Transform generatedRoomsRoot;
    [SerializeField, Min(1)] private int maxGenerationAttempts = 2000;

    [Header("Section size")]
    [SerializeField, Min(1)] public int GenerateRoomsAhead = 5;
    [SerializeField, Min(1)] private int rollbackSections = 3;

    [SerializeField] private bool generateOnStart = true;
    private int requestedRoomCount;

    public int RoomsGenerated => roomsGenerated;

    RoomDecider decider;
    RoomPlacer placer;

    void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        decider = GetComponent<RoomDecider>();
        placer = GetComponent<RoomPlacer>();
        if (generateOnStart) StartGeneration();
    }

    public void StartGeneration()
    {
        CreateGeneratedRoot();
        roomsGenerated = 0;
        requestedRoomCount = 0;
        GenerateFirstRoom();

    }

    void CreateGeneratedRoot()
    {
        if (generatedRoomsRoot == null)
        {
            var rootObject = new GameObject("Generated Rooms");
            generatedRoomsRoot = rootObject.transform;
            generatedRoomsRoot.SetParent(transform, false);
        }
    }

    public void GenerateFirstRoom()
    {
        bool placed = placer.PlaceFirstRoom(decider.GetRoomCandidates(), generatedRoomsRoot);
        roomsGenerated = placer.RoomsPlaced;
        requestedRoomCount = roomsGenerated;
        if (!placed) Debug.LogError("No eligible starting room could be placed.", this);

    }

    /// <summary>Generate the next rooms during gameplay. Call this when the player is nearing the end of the current section.</summary>
    public bool GenerateNextSection()
    {
        if (requestedRoomCount >= roomCount && roomsGenerated >= roomCount) return true;
        if (requestedRoomCount <= roomsGenerated)
            requestedRoomCount = Mathf.Min(roomCount, roomsGenerated + Mathf.Max(1, GenerateRoomsAhead));

        bool generated = placer.GenerateRoomsUpToCount(
            decider.GetRoomCandidates(), generatedRoomsRoot, requestedRoomCount, maxGenerationAttempts);
        roomsGenerated = placer.RoomsPlaced;
        if (!generated)
            RollbackFailedSection();
        return generated;
    }

    private void RollbackFailedSection()
    {
        int roomsPerSection = Mathf.Max(1, GenerateRoomsAhead);
        int roomsToRollback = roomsPerSection * rollbackSections;
        int removedRooms = placer.RollbackLastRooms(roomsToRollback);
        roomsGenerated = placer.RoomsPlaced;

        Debug.LogWarning(
            $"Could not complete generation after {maxGenerationAttempts} attempts. " +
            $"Rolled back {removedRooms} rooms ({rollbackSections} sections) and will retry from there.", this);
    }
    void Update()
    {
        if (Keyboard.current.gKey.wasPressedThisFrame)
        {
            GenerateNextSection();
        }
    }
}
