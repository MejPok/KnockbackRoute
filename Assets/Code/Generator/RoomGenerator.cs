using System.Collections.Generic;
using UnityEngine;

/// <summary>Unity entry point for starting and incrementally extending a generated dungeon.</summary>
[RequireComponent(typeof(RoomDecider))]
[RequireComponent(typeof(RoomPlacer))]

public class RoomGenerator : MonoBehaviour
{
    public static RoomGenerator Instance;

    public int roomCount = 40;
    public int roomsGenerated = 0;
    [SerializeField] public Transform generatedRoomsRoot;
    [SerializeField, Min(1)] private int maxGenerationAttempts = 2000;

    [Header("Section size")]
    [SerializeField, Min(1)] public int GenerateRoomsAhead = 5;

    private int originalRollbackSections = 3;
    [Min(1)] private int rollbackSections { get => originalRollbackSections + addedRollbackSections; set => originalRollbackSections = value; }
    public int maxAddedRollbackSections = 10;
    public int addedRollbackSections = 0;

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
        RoomDefinition firstRoomCandidate = decider.GetRoomCandidateByTag("SpawnRoom");

        bool placed = false;
        placed = placer.PlaceFirstRoom(firstRoomCandidate, generatedRoomsRoot);

        roomsGenerated = placer.RoomsPlaced;
        requestedRoomCount = roomsGenerated;

        if (!placed) Debug.LogError("No eligible starting room could be placed.", this);

    }

    public void GenerateLastRoom()
    {
        RoomDefinition lastRoomCandidate = decider.GetRoomCandidateByTag("LastRoom");

        bool placed = false;
        if (placer.lastExit != null)
        {
            placed = placer.PlaceFirstRoom(lastRoomCandidate, placer.lastExit.transform);

        }

        if (!placed) Debug.LogError("No eligible starting room could be placed.", this);

    }

    public int contractNumber;
    public int contractPhase;

    public bool GenerateNextSection()
    {
        if (placer == null || placer.RoomsPlaced == 0) return false;
        if (roomsGenerated >= roomCount) return true;

        int roomsForStep = decider.StepRoomCount(contractPhase);
        if (roomsForStep <= 0) return false;

        int startingRoomCount = placer.RoomsPlaced;
        int startingContractNumber = contractNumber;
        int startingContractPhase = contractPhase;

        int targetRoomCount = Mathf.Min(roomCount, startingRoomCount + roomsForStep);
        bool generated = placer.GenerateRoomsUpToCount(
            decider.ContractStepCandidates(contractPhase), generatedRoomsRoot, targetRoomCount,
            maxGenerationAttempts, startingRoomCount);

        roomsGenerated = placer.RoomsPlaced;
        if (!generated)
        {
            RollbackFailedSection();
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
        if (firstSectionToKeep == 0) Debug.LogWarning("Rollback sections exceeds generated sections. Rollback might remove all generated rooms.", this);

        int minimumRoomsToKeep = 1;
        for (int i = 0; i < firstSectionToKeep; i++)
            minimumRoomsToKeep += generatedSections[i].roomCount;

        return Mathf.Min(minimumRoomsToKeep, placer.RoomsPlaced);
    }

    int successfulRollbacks = 0;
    private void RollbackFailedSection()
    {
        int minimumRoomsToKeep = GetRollbackBoundary(out int firstSectionToKeep);
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

    int lastFailedRoomRollback = 0;
    int failedRollback = 0;
    public float StepSpawnCD = 0.5f;
    float timer = 0f;

    bool assignedIds = false;
    public CorruptionBehaviour corruption;
    bool reachedRoomCount = false;
    void Update()
    {

        timer += Time.deltaTime;
        if (timer >= StepSpawnCD)
        {
            timer = 0;
            bool success = GenerateNextSection();
            if (success)
            {
                successfulRollbacks++;
            }
            else
            {
                failedRollback++;
                successfulRollbacks = 0;
                lastFailedRoomRollback = roomsGenerated;
            }
            
            if (failedRollback >= 10) addedRollbackSections = Mathf.Min(addedRollbackSections + 1, maxAddedRollbackSections);

            if(successfulRollbacks > rollbackSections)
            {
                addedRollbackSections = Mathf.Max(0, addedRollbackSections - 1);
                failedRollback = 0;
            }
        }


        if (roomsGenerated >= roomCount && !reachedRoomCount)
        {
            reachedRoomCount = true;
            GetComponent<WinPlacer>().PlaceWinObject(placer.lastExit.transform);
            mapLoadUI.SetActive(false);
            corruption.gameObject.SetActive(true);
            if (assignedIds == false)
            {
                SignalIdGiver.StartAssigning();
                assignedIds = true;
                Debug.Log("Assigning signal ids");
            }
            GenerateLastRoom();
        }
    }

    GameObject mapLoadUI;
    public void SetMapLoadUI(GameObject mapLoadUI)
    {
        this.mapLoadUI = mapLoadUI;
    }
}
