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
    
    private System.Random random;

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
        Shuffler.Shuffle(roomsClone);

        return roomsClone.ToArray();
    }



    List<ContractQualificator> contractQualifaction = new List<ContractQualificator>();
    public bool contractFinished;

    
    bool ChooseNewContract()
    {
        contractQualifaction.Add(new ContractQualificator(this));
        if (contractQualifaction[contractQualifaction.Count - 1].ChooseNewContract())
        {
            if(contractQualifaction.Count() == 1)
            {
                RoomGenerator.Instance.contractNumber++;
            }
            
            
            return true;
        } else
        {
            return false;
        }
        
    }

    /*
    jetli neexistuje contract
        Vytvorit moznej contract

        ziskat vsechny mozne mistnosti pro nej

    vracet random mozne mistnosti pri getovani mistnosti

    */

    public RoomDefinition[] ContractStepCandidates(int step)
    {
        if(RoomGenerator.Instance.contractNumber >= contractQualifaction.Count())
        {
            Debug.Log($"Neexistuje contract pro {RoomGenerator.Instance.contractNumber} ve vygenerovanych contractech");
        }
        return contractQualifaction[RoomGenerator.Instance.contractNumber].qualifiedRooms[step].ToArray();
    }

    public bool CheckContractEnd(int phase)
    {
        if(phase >= contractQualifaction[RoomGenerator.Instance.contractNumber].qualifiedRooms.Count)
        {
            return ChooseNewContract();
        }
        return false;
    }

    public int StepRoomCount(int phase)
    {

        return contractQualifaction[RoomGenerator.Instance.contractNumber].contractSteps[phase].getRealCount();
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
