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

    public static List<SignalPairing> signalPairs = new List<SignalPairing>();
    public static List<SignalPairing> WaitingForId = new List<SignalPairing>();

    public static void WaitForId(SignalPairing unit)
    {
        WaitingForId.Add(unit);
    }


    public static void StartAssigning()
    {
        Dictionary<string, int> assigningTypeCount = new Dictionary<string, int>();

        foreach (SignalPairing unit in WaitingForId)
        {
            if (unit == null) return;

            if (assigningTypeCount.ContainsKey(unit.possibleSignalId))
            {
                unit.assignedId = assigningTypeCount[unit.possibleSignalId] / 2;
                unit.waitingForId = false;
                assigningTypeCount[unit.possibleSignalId]++;
            }
            else
            {
                unit.assignedId = 0;
                unit.waitingForId = false;
                assigningTypeCount[unit.possibleSignalId] = 1;
            }

            signalPairs.Add(unit);
            Debug.Log($"Assigned ID {unit.assignedId} to signal {unit.possibleSignalId}");
        }




    }
}