using System;
using Mono.Cecil;
using System.Collections.Generic;
using UnityEngine;

public class SignalManager
{
    public static int signalId;
    public static event SignalHandler NewSignalTriggered;
    public delegate void SignalHandler(int e);

    public static void NewSignal(int id)
    {
        NewSignalTriggered.Invoke(id);
    }

}

public static class SignalIdGiver
{
    private static int nextId = 1;
    private static readonly Dictionary<string, int> waiting = new();

    public static int GetId(string kind)
    {
        if (waiting.TryGetValue(kind, out int id))
        {
            waiting.Remove(kind); // This ID has now been given to two callers.
            return id;
        }

        id = nextId++;
        waiting[kind] = id;
        return id;
    }
}