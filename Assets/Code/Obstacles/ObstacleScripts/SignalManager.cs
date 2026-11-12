using System;
using System.Collections.Generic;
using UnityEngine;

public class SignalManager
{
    public static event SignalHandler NewSignalTriggered;
    public delegate void SignalHandler(string possible, int e);

    public static void NewSignal(string possible, int id)
    {
        NewSignalTriggered?.Invoke(possible, id);
    }
}

public static class SignalIdGiver
{
    public static List<SignalPairing> signalPairs = new List<SignalPairing>();
    public static List<SignalPairing> WaitingForId = new List<SignalPairing>();

    public static void WaitForId(SignalPairing unit)
    {
        if (unit == null || WaitingForId.Contains(unit)) return;

        WaitingForId.Add(unit);
        AssignedEverything = false;
    }

    public static bool AssignedEverything = false;

    public static void StartAssigning()
    {
        // Destroyed rooms can leave Unity's fake-null objects in this list until
        // their OnDestroy callback has run. Remove them without aborting the pass.
        WaitingForId.RemoveAll(unit => unit == null);

        Dictionary<string, int> assigningTypeCount = new Dictionary<string, int>();

        foreach (SignalPairing unit in WaitingForId)
        {
            if (unit == null) continue;

            if (assigningTypeCount.ContainsKey(unit.possibleSignalId))
            {
                unit.assignedId = assigningTypeCount[unit.possibleSignalId] / 2;
                assigningTypeCount[unit.possibleSignalId]++;
            }
            else
            {
                unit.assignedId = 0;
                assigningTypeCount[unit.possibleSignalId] = 1;
            }

            unit.waitingForId = false;
            signalPairs.Add(unit);
            Debug.Log($"Assigned ID {unit.assignedId} to signal {unit.possibleSignalId}");
        }

        WaitingForId.Clear();
        AssignedEverything = true;
    }
}