using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Linq;
public class RoomDecider : MonoBehaviour
{
    [SerializeField] private RoomDefinition[] rooms;
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
    }

    private bool HasRoomPool()
    {
        if (rooms != null && rooms.Length > 0) return true;
        Debug.LogError("RoomDecider needs at least one RoomDefinition.", this);
        return false;
    }

    public RoomDefinition[] GetRoomCandidates()
    {
        if (!HasRoomPool()) return Array.Empty<RoomDefinition>();
        return (RoomDefinition[])rooms.Clone();
    }

    #nullable enable
    public RoomDefinition? ChooseRandomRoom(int difficulty = 1)
    {
        if (HasRoomPool())
        {
            var candidate = ChooseRandomCandidate(rooms.ToList());
            while(candidate.difficulty > difficulty)
            {
                candidate = ChooseRandomCandidate(rooms.ToList());
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
