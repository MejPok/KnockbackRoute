using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Pairs each button with the next unpaired door that uses the same authored channel.</summary>
internal sealed class RoomSignalPairing
{
    private readonly int seed;
    private readonly Dictionary<string, Queue<string>> pendingDoors = new Dictionary<string, Queue<string>>(StringComparer.Ordinal);
    private int nextInstanceId;

    public RoomSignalPairing(int seed) => this.seed = seed;

    public Dictionary<string, int> BuildAvailability()
    {
        var result = new Dictionary<string, int>(StringComparer.Ordinal);
        foreach (KeyValuePair<string, Queue<string>> pair in pendingDoors)
            result[pair.Key] = pair.Value.Count;
        return result;
    }

    public bool CanPlaceDoors(GameObject roomPrefab, Dictionary<string, int> availability)
    {
        var required = new Dictionary<string, int>(StringComparer.Ordinal);
        foreach (ButtonDoor door in roomPrefab.GetComponentsInChildren<ButtonDoor>(true))
        {
            if (string.IsNullOrWhiteSpace(door.signalId)) return false;
            required.TryGetValue(door.signalId, out int count);
            required[door.signalId] = count + 1;
        }
        foreach (KeyValuePair<string, int> door in required)
            if (!availability.TryGetValue(door.Key, out int count) || count < door.Value) return false;
        return true;
    }

    public void AddRoomToAvailability(GameObject room, Dictionary<string, int> availability)
    {
        foreach (ButtonDoor door in room.GetComponentsInChildren<ButtonDoor>(true))
            availability[door.signalId]--;
        foreach (ButtonBullet button in room.GetComponentsInChildren<ButtonBullet>(true))
        {
            if (string.IsNullOrWhiteSpace(button.signalId)) continue;
            availability.TryGetValue(button.signalId, out int count);
            availability[button.signalId] = count + 1;
        }
    }

    public void CommitRoom(GameObject room)
    {
        foreach (ButtonDoor door in room.GetComponentsInChildren<ButtonDoor>(true))
        {
            string channel = door.signalId;
            if (string.IsNullOrWhiteSpace(channel)) continue;
            string pairedSignal = Dequeue(channel) ?? CreateInstanceSignal(channel);
            door.SetSignalId(pairedSignal);
        }

        foreach (ButtonBullet button in room.GetComponentsInChildren<ButtonBullet>(true))
        {
            string channel = button.signalId;
            if (string.IsNullOrWhiteSpace(channel)) continue;
            string instanceSignal = CreateInstanceSignal(channel);
            button.SetSignalId(instanceSignal);
            if (!pendingDoors.TryGetValue(channel, out Queue<string> queue))
                pendingDoors[channel] = queue = new Queue<string>();
            queue.Enqueue(instanceSignal);
        }
    }

    public void Reset()
    {
        pendingDoors.Clear();
        nextInstanceId = 0;
        RoomSignalBus.Reset();
    }

    private string Dequeue(string channel)
    {
        if (!pendingDoors.TryGetValue(channel, out Queue<string> queue) || queue.Count == 0) return null;
        string signal = queue.Dequeue();
        if (queue.Count == 0) pendingDoors.Remove(channel);
        return signal;
    }

    private string CreateInstanceSignal(string channel) => $"{seed}:{channel}:{nextInstanceId++}";
}
