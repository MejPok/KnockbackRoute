using System;
using System.Collections.Generic;

/// <summary>Small run-local signal channel for wiring buttons to doors across room prefabs.</summary>
public static class RoomSignalBus
{
    private static readonly HashSet<string> raisedSignals = new HashSet<string>(StringComparer.Ordinal);
    private static readonly Dictionary<string, Action> listeners = new Dictionary<string, Action>(StringComparer.Ordinal);

    public static void Subscribe(string signal, Action listener)
    {
        if (string.IsNullOrWhiteSpace(signal) || listener == null) return;
        if (!listeners.TryGetValue(signal, out Action handlers))
            listeners[signal] = handlers = null;
        handlers += listener;
        listeners[signal] = handlers;
        if (raisedSignals.Contains(signal)) listener();
    }

    public static void Unsubscribe(string signal, Action listener)
    {
        if (string.IsNullOrWhiteSpace(signal) || listener == null || !listeners.TryGetValue(signal, out Action handlers)) return;
        handlers -= listener;
        if (handlers == null) listeners.Remove(signal);
        else listeners[signal] = handlers;
    }

    public static void Raise(string signal)
    {
        if (string.IsNullOrWhiteSpace(signal) || !raisedSignals.Add(signal)) return;
        if (listeners.TryGetValue(signal, out Action handlers)) handlers?.Invoke();
    }

    public static void Reset()
    {
        raisedSignals.Clear();
        listeners.Clear();
    }
}
