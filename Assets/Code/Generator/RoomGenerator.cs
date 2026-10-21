using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>Unity entry point for starting and incrementally extending a generated dungeon.</summary>
public class RoomGenerator : MonoBehaviour
{
    [Header("Room pool")]
    [SerializeField] private RoomDefinition[] rooms;
    [SerializeField] private RoomContract contract;
    [SerializeField] private RoomContract[] additionalContracts;
    [SerializeField, Min(1)] private int roomCount = 40;
    [SerializeField] private int roomsGenerated;
    [SerializeField] private Transform generatedRoomsRoot;
    [SerializeField, Min(1)] private int maxGenerationAttempts = 2000;

    [Header("Section size")]
    [SerializeField, Min(1)] public int GenerateRoomsAhead = 5;

    [Header("Seed")]
    [SerializeField] private int seed;
    [SerializeField] private bool createSeedOnStart = true;
    [SerializeField] private bool generateOnStart = true;

    public int CurrentSeed { get; private set; }
    public int RoomsGenerated => roomsGenerated;

    private RoomPlacementSolver placementSolver;

    private void Start()
    {
        if (generateOnStart) Generate();
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
            GenerateNextSection();
    }

    /// <summary>Starts a new run with a fresh or configured seed.</summary>
    public void Generate()
    {
        CurrentSeed = createSeedOnStart ? Environment.TickCount : seed;
        seed = CurrentSeed;
        ResetRun();
        GenerateNextSection();
    }

    /// <summary>Starts a repeatable new run using the configured seed.</summary>
    public void GenerateWithSeed()
    {
        CurrentSeed = seed;
        ResetRun();
        GenerateNextSection();
    }

    /// <summary>Compatibility entry point; appends the next section.</summary>
    public void GenerateNeededRooms() => GenerateNextSection();

    /// <summary>Appends one section while preserving seed, contracts, and connector state.</summary>
    public void GenerateNextSection()
    {
        if (!HasRoomPool()) return;
        EnsurePlacementSolver();

        bool generated = placementSolver.GenerateSection(roomCount, GenerateRoomsAhead, maxGenerationAttempts,
            out int addedCount, out string failureReason);
        roomsGenerated = placementSolver.RoomsGenerated;

        if (!generated)
        {
            Debug.LogWarning(failureReason, this);
            return;
        }

        if (addedCount == 0)
        {
            Debug.Log("The dungeon has reached its configured room count.", this);
            return;
        }

        Debug.Log($"Generated section: {addedCount} rooms; total {roomsGenerated}/{roomCount}, seed {CurrentSeed}.", this);
    }

    private bool HasRoomPool()
    {
        if (rooms != null && rooms.Length > 0) return true;
        Debug.LogError("RoomGenerator needs at least one RoomDefinition.", this);
        return false;
    }

    private void ResetRun()
    {
        placementSolver = null;
        roomsGenerated = 0;
        EnsurePlacementSolver();
        placementSolver.Reset();
    }

    private void EnsurePlacementSolver()
    {
        if (placementSolver != null) return;
        if (generatedRoomsRoot == null)
        {
            var rootObject = new GameObject("Generated Rooms");
            generatedRoomsRoot = rootObject.transform;
            generatedRoomsRoot.SetParent(transform, false);
        }

        var contracts = new List<RoomContract> { contract };
        if (additionalContracts != null) contracts.AddRange(additionalContracts);
        var selection = new RoomSelectionPolicy(rooms, contracts.ToArray(), roomCount);
        var signalPairing = new RoomSignalPairing(CurrentSeed);
        placementSolver = new RoomPlacementSolver(
            generatedRoomsRoot,
            selection,
            signalPairing,
            new System.Random(CurrentSeed));
    }
}
