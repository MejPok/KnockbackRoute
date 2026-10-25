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

    private struct GeneratedSection
    {
        public int roomCount;
        public int contractNumber;
        public int contractPhase;

        public GeneratedSection(int roomCount, int contractNumber, int contractPhase)
        {
            this.roomCount = roomCount;
            this.contractNumber = contractNumber;
            this.contractPhase = contractPhase;
        }
    }

    private readonly List<GeneratedSection> generatedSections = new List<GeneratedSection>();
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
        contractNumber = 0;
        contractPhase = 0;
        generatedSections.Clear();

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
        bool placed = placer.PlaceFirstRoom(decider.GetRandomRoomCandidates(), generatedRoomsRoot);

        roomsGenerated = placer.RoomsPlaced;
        requestedRoomCount = roomsGenerated;

        if (!placed) Debug.LogError("No eligible starting room could be placed.", this);

    }

    /// <summary>Generate the next rooms during gameplay. Call this when the player is nearing the end of the current section.</summary>
    public bool GenerateNextRandomSection()
    {
        if (requestedRoomCount >= roomCount && roomsGenerated >= roomCount) return true;
        if (requestedRoomCount <= roomsGenerated)
            requestedRoomCount = Mathf.Min(roomCount, roomsGenerated + Mathf.Max(1, GenerateRoomsAhead));

        int startingRoomCount = placer.RoomsPlaced;
        int startingContractNumber = contractNumber;
        int startingContractPhase = contractPhase;
        int minimumRoomsToKeep = GetRollbackBoundary(out int firstSectionToKeep);

        bool generated = placer.GenerateRoomsUpToCount(
            decider.GetRandomRoomCandidates(), generatedRoomsRoot, requestedRoomCount,
            maxGenerationAttempts, minimumRoomsToKeep);

        roomsGenerated = placer.RoomsPlaced;
        if (!generated)
        {
            RollbackFailedSection(minimumRoomsToKeep, firstSectionToKeep);
            return false;
        }

        generatedSections.Add(new GeneratedSection(
            roomsGenerated - startingRoomCount, startingContractNumber, startingContractPhase));
        return true;
    }

    public int contractNumber;
    public int contractPhase;

    public bool GenerateNextSection()
    {
        if (roomsGenerated >= roomCount) return true;

        int roomsForStep = decider.StepRoomCount(contractPhase);
        if (roomsForStep <= 0) return false;

        int startingRoomCount = placer.RoomsPlaced;
        int startingContractNumber = contractNumber;
        int startingContractPhase = contractPhase;
        int minimumRoomsToKeep = GetRollbackBoundary(out int firstSectionToKeep);
        int targetRoomCount = Mathf.Min(roomCount, startingRoomCount + roomsForStep);
        bool generated = placer.GenerateRoomsUpToCount(
            decider.ContractStepCandidates(contractPhase), generatedRoomsRoot, targetRoomCount,
            maxGenerationAttempts, minimumRoomsToKeep);

        roomsGenerated = placer.RoomsPlaced;
        if (!generated)
        {
            RollbackFailedSection(minimumRoomsToKeep, firstSectionToKeep);
            return false;
        }

        generatedSections.Add(new GeneratedSection(
            roomsGenerated - startingRoomCount, startingContractNumber, startingContractPhase));
        contractPhase++;

        if (decider.CheckContractEnd(contractPhase))
            contractPhase = 0;

        return true;
    }

    private int GetRollbackBoundary(out int firstSectionToKeep)
    {
        firstSectionToKeep = Mathf.Max(0, generatedSections.Count - Mathf.Max(1, rollbackSections));
        int minimumRoomsToKeep = 1;
        for (int i = 0; i < firstSectionToKeep; i++)
            minimumRoomsToKeep += generatedSections[i].roomCount;

        return Mathf.Min(minimumRoomsToKeep, placer.RoomsPlaced);
    }

    private void RollbackFailedSection(int minimumRoomsToKeep, int firstSectionToKeep)
    {
        int roomsToRemove = Mathf.Max(0, placer.RoomsPlaced - minimumRoomsToKeep);
        int removedRooms = placer.RollbackLastRooms(roomsToRemove);
        int removedSections = generatedSections.Count - firstSectionToKeep;

        if (removedSections > 0)
        {
            GeneratedSection firstRemoved = generatedSections[firstSectionToKeep];
            contractNumber = firstRemoved.contractNumber;
            contractPhase = firstRemoved.contractPhase;
            generatedSections.RemoveRange(firstSectionToKeep, removedSections);
        }

        roomsGenerated = placer.RoomsPlaced;
        requestedRoomCount = roomsGenerated;

        Debug.LogWarning(
            $"Could not complete generation after {maxGenerationAttempts} attempts. " +
            $"Rolled back {removedRooms} rooms across {removedSections} sections; retrying from the preserved map.", this);
    }
    public float StepSpawnCD = 0.5f;
    float timer = 0f;
    
    void Update()
    {
        if (Keyboard.current.gKey.wasPressedThisFrame)
        {
            GenerateNextSection();
        }

        timer += Time.deltaTime;
        if(timer >= StepSpawnCD)
        {
            timer = 0;
            GenerateNextSection();
        }

        if(roomsGenerated == roomCount)
        {
            GetComponent<WinPlacer>().PlaceWinObject(placer.lastExit.transform); 
        }
    }
}
