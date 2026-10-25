using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Linq;
using JetBrains.Annotations;
public class RoomDecider : MonoBehaviour
{
    public int Difficulty;
    [SerializeField] public RoomDefinition[] Rooms;
    [SerializeField] private bool createSeedOnStart = true;
    public int CurrentSeed { get; private set; }
    [SerializeField] private int seed;

    [Serialize]
    public RoomContract[] possibleContracts;
    
    public System.Random random;

    public void CreateNewSeed()
    {
        CurrentSeed = createSeedOnStart ? Environment.TickCount : seed;
        seed = CurrentSeed;
    }
    void Awake()
    {
        CreateNewSeed();
        random = new System.Random(CurrentSeed);
        ChooseNewContract();
    }

    private bool HasRoomPool()
    {
        if (Rooms != null && Rooms.Length > 0) return true;
        Debug.LogError("RoomDecider needs at least one RoomDefinition.", this);
        return false;
    }

    public RoomDefinition[] GetRandomRoomCandidates()
    {
        if (!HasRoomPool()) return Array.Empty<RoomDefinition>();

        var roomsClone = ((RoomDefinition[])Rooms.Clone()).ToList();
        Shuffler.Shuffle(roomsClone, random);

        return roomsClone.ToArray();
    }



    List<ContractQualificator> contractQualifaction = new List<ContractQualificator>();
    public bool contractFinished;

    
    bool ChooseNewContract()
    {
        var qualifier = new ContractQualificator(this);

        if (!qualifier.ChooseNewContract())
            return false;

        contractQualifaction.Add(qualifier);
        return true;
        
    }

    private bool TryGetCurrentContract(out ContractQualificator qualifier)
    {
        qualifier = null;

        if (contractQualifaction == null ||
            RoomGenerator.Instance == null ||
            RoomGenerator.Instance.contractNumber < 0 ||
            RoomGenerator.Instance.contractNumber >= contractQualifaction.Count)
        {
            Debug.LogError("No valid contract is available.", this);
            return false;
        }

        qualifier = contractQualifaction[RoomGenerator.Instance.contractNumber];
        return qualifier != null && qualifier.qualifiedRooms != null;
    }

    /*
    jetli neexistuje contract
        Vytvorit moznej contract

        ziskat vsechny mozne mistnosti pro nej

    vracet random mozne mistnosti pri getovani mistnosti

    */

    public RoomDefinition[] ContractStepCandidates(int step)
    {
        if (!TryGetCurrentContract(out var qualifier) ||
        step < 0 || step >= qualifier.qualifiedRooms.Count)
        return Array.Empty<RoomDefinition>();

        return qualifier.qualifiedRooms[step].ToArray();
    }

   public bool CheckContractEnd(int phase)
    {
        if (!TryGetCurrentContract(out var qualifier))
            return false;

        if (phase < qualifier.qualifiedRooms.Count)
            return false;

        RoomGenerator.Instance.contractNumber++;
        return ChooseNewContract();
    }


    public int StepRoomCount(int phase)
    {
        return contractQualifaction[RoomGenerator.Instance.contractNumber].roomsPerStep[phase];
    }


    #nullable enable
    public RoomDefinition? ChooseRandomRoom(int difficulty = 1)
    {
        if (HasRoomPool())
        {
            var candidate = ChooseRandomCandidate(Rooms.ToList());
            while(candidate.difficulty > difficulty)
            {
                candidate = ChooseRandomCandidate(Rooms.ToList());
            }
            
            return candidate;
        }

        return null;
    }


    private RoomDefinition ChooseRandomCandidate(List<RoomDefinition> candidates)
    {
        if(random == null)
        {
            Debug.LogError("Random isnt created");
        }
        return candidates[random.Next(candidates.Count -1)];
    }

}
