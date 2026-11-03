using System;
using System.Collections.Generic;
using UnityEngine;

public class SignalManager
{
    public static event SignalHandler NewSignalTriggered;
    public delegate void SignalHandler(string possible,int e);

    public static void NewSignal(string possible, int id)
    {
        NewSignalTriggered.Invoke(possible,id);
    }

}

public static class SignalIdGiver
{
    private static int nextId = 1;
    private static readonly Dictionary<string, int> waiting = new();

    public static List<SignalPairing> signalPairs = new List<SignalPairing>();
    public static List<SignalPairing> WaitingForId = new List<SignalPairing>();


    public static void WaitForId(SignalPairing unit)
    {
        WaitingForId.Add(unit);
    }
    public static int GetId(SignalPairing unit)
    {

        int sameTypeCount = 0;
        for(int i = 0; i < signalPairs.Count; i++)
        {
            if(signalPairs[i].possibleSignalId == unit.possibleSignalId)
            {
                sameTypeCount++;
            }
        }

        int newId = (int)(sameTypeCount / 2);
        signalPairs.Add(unit);
        return newId;

    }

    public static void StartAssigning()
    {
        Dictionary<string, int> assigningTypeCount = new Dictionary<string, int>();

        foreach (SignalPairing unit in WaitingForId)
        {
            if (unit == null) return;

            if (assigningTypeCount.ContainsKey(unit.possibleSignalId))
            {
                signalPairs.Add(unit);
                unit.assignedId = assigningTypeCount[unit.possibleSignalId] / 2;
                assigningTypeCount[unit.possibleSignalId]++;
                Debug.Log($"Assigned ID {unit.assignedId} to signal {unit.possibleSignalId}");
            }
            else
            {
                assigningTypeCount[unit.possibleSignalId] = 1;
            }
        }




    }
}