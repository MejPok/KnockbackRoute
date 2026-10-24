using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class ContractQualificator
{
    public RoomDecider decider;
    public ContractQualificator(RoomDecider decider)
    {
        this.decider = decider;
    }
    RoomContract chosenContract;

    public RoomContract.Step[] contractSteps;
    public List<List<RoomDefinition>> qualifiedRooms = new List<List<RoomDefinition>>();
    public int contractDepth;

    List<RoomDefinition> createdContractRooms = new List<RoomDefinition>();
    public bool ChooseNewContract()
    {
        Shuffler.Shuffle(decider.possibleContracts.ToList());
        foreach(var contract in decider.possibleContracts)
        {
            if(Math.Abs(decider.Difficulty - contract.ExpectedDifficulty) <= 2)
            {
                chosenContract = contract;
                contractSteps = chosenContract.steps;
                contractDepth = 0;
                decider.contractFinished = false;
                chosenContract.GetAverageContractDifficulty();
                break;
            }
        }
        if(chosenContract == null)
        {
            return false;
        }
        return CreateAllQualifiedRooms();
    }
    public bool CreateAllQualifiedRooms()
    {
        for(int i = 0; i < contractSteps.Length; i++)
        {
            RoomContract.Step criteria = contractSteps[i];

            List<RoomDefinition> possibleRooms = new List<RoomDefinition>();

            foreach(var room in decider.Rooms)
            {

                if(DefinitionQualified(room, criteria))
                {
                    possibleRooms.Add(room);
                }
                
            }

            qualifiedRooms.Add(possibleRooms);

        }

        if(qualifiedRooms.Count == contractSteps.Length)
        {
            return true;

        } else
        {
            return false;
        }
    }

    public bool DefinitionQualified(RoomDefinition definition, RoomContract.Step criteria)
    {
        if(criteria.requiredTag == "") //checkujem zdali nema byt special
        {
            if(definition.tags.Length >= 1) //nechceme davat klasicke mistnosti jako neklasicke
            {
                return false;
            }

            if(definition.difficulty >= criteria.minimumDifficulty && definition.difficulty <= criteria.maximumDifficulty)
            {
                return true;
            }
                
        } else
        {
            if (definition.tags.Contains(criteria.requiredTag)) // je special?
            {
                if(definition.difficulty >= criteria.minimumDifficulty && definition.difficulty <= criteria.maximumDifficulty || criteria.maximumDifficulty == 0)
                {
                    return true;
                }
            } 
        }

        return false;
    }
}
