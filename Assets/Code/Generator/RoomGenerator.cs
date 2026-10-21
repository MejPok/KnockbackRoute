using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>Unity entry point for starting and incrementally extending a generated dungeon.</summary>
[RequireComponent(typeof(RoomDecider))]
[RequireComponent(typeof(RoomPlacer))]

public class RoomGenerator : MonoBehaviour
{
    [SerializeField, Min(1)] private int roomCount = 40;
    [SerializeField] private int roomsGenerated;
    [SerializeField] private Transform generatedRoomsRoot;
    [SerializeField, Min(1)] private int maxGenerationAttempts = 2000;

    [Header("Section size")]
    [SerializeField, Min(1)] public int GenerateRoomsAhead = 5;

    [SerializeField] private bool generateOnStart = true;

    public int RoomsGenerated => roomsGenerated;

    RoomDecider decider;
    RoomPlacer placer;
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
        #nullable enable
        RoomDefinition? firstRoom = decider.ChooseRandomRoom();
        if(firstRoom == null)   return;

        placer.PlaceFirstRoom(firstRoom, generatedRoomsRoot);
        roomsGenerated = 1;


    }

    /// <summary>Generate the next rooms during gameplay. Call this when the player is nearing the end of the current section.</summary>
    public bool GenerateNextSection()
    {
        if (roomsGenerated >= roomCount) return true;
        int roomsToAdd = Mathf.Min(Mathf.Max(1, GenerateRoomsAhead), roomCount - roomsGenerated);
        bool generated = placer.GenerateRoomsWithBacktracking(decider.GetRoomCandidates(), generatedRoomsRoot, roomsToAdd);
        if (generated) roomsGenerated += roomsToAdd;
        return generated;
    }

/*
    private void EnsurePlacementSolver()
    {
        

        var contracts = new List<RoomContract> { contract };
        if (additionalContracts != null) contracts.AddRange(additionalContracts);
        var selection = new RoomSelectionPolicy(rooms, contracts.ToArray(), roomCount);
        var signalPairing = new RoomSignalPairing(CurrentSeed);
        placementSolver = new RoomPlacementSolver(
            generatedRoomsRoot,
            selection,
            signalPairing,
            new System.Random(CurrentSeed));
    } */
}
