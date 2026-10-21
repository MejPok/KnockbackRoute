using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Filters the room pool using connector, contract, and difficulty rules.</summary>
internal sealed class RoomSelectionPolicy
{
    private readonly RoomDefinition[] rooms;
    private readonly RoomContract[] contracts;
    private readonly int roomCount;
    private readonly int maximumPoolDifficulty;

    public RoomSelectionPolicy(RoomDefinition[] rooms, RoomContract[] contracts, int roomCount)
    {
        this.rooms = rooms ?? Array.Empty<RoomDefinition>();
        this.contracts = contracts ?? Array.Empty<RoomContract>();
        this.roomCount = roomCount;
        foreach (RoomDefinition room in this.rooms)
            if (room != null) maximumPoolDifficulty = Mathf.Max(maximumPoolDifficulty, room.difficulty);
    }

    public List<RoomDefinition> GetCandidates(RoomConnector.ConnectionType? requiredConnector, int roomIndex)
    {
        var candidates = new List<RoomDefinition>();
        RoomContract.Step step = GetContractStep(roomIndex);
        int targetDifficulty = Mathf.Min(maximumPoolDifficulty, roomIndex / 4);

        foreach (RoomDefinition room in rooms)
        {
            if (!HasRequiredConnectors(room, requiredConnector, roomIndex)) continue;
            if (!MatchesDifficulty(room, step, targetDifficulty)) continue;
            if (!MatchesContractTag(room, step)) continue;

            // Special rooms are available only at a contract step that explicitly asks for Special.
            if (HasTag(room, "Special") &&
                (step == null || !string.Equals(step.requiredTag, "Special", StringComparison.OrdinalIgnoreCase)))
                continue;

            candidates.Add(room);
        }
        return candidates;
    }

    private bool HasRequiredConnectors(RoomDefinition room, RoomConnector.ConnectionType? requiredConnector, int roomIndex)
    {
        if (room == null || room.prefab == null) return false;
        if (requiredConnector.HasValue && FindConnector(room.prefab, requiredConnector.Value) == null) return false;
        if ((requiredConnector == RoomConnector.ConnectionType.Entry ||
             (!requiredConnector.HasValue && roomIndex == 0 && roomCount > 1)) &&
            FindConnector(room.prefab, RoomConnector.ConnectionType.Exit) == null) return false;
        return true;
    }

    private static bool MatchesDifficulty(RoomDefinition room, RoomContract.Step step, int targetDifficulty)
    {
        if (step != null)
            return room.difficulty >= step.minimumDifficulty && room.difficulty <= step.maximumDifficulty;
        return room.difficulty <= targetDifficulty + 1;
    }

    private static bool MatchesContractTag(RoomDefinition room, RoomContract.Step step)
    {
        return step == null || string.IsNullOrWhiteSpace(step.requiredTag) || HasTag(room, step.requiredTag);
    }

    private RoomContract.Step GetContractStep(int roomIndex)
    {
        int remainingIndex = roomIndex;
        foreach (RoomContract contract in contracts)
        {
            if (contract == null) continue;
            if (remainingIndex < contract.StepCount) return contract.GetStep(remainingIndex);
            remainingIndex -= contract.StepCount;
        }
        return null;
    }

    private static bool HasTag(RoomDefinition room, string requiredTag)
    {
        if (room.tags == null) return false;
        foreach (string tag in room.tags)
            if (string.Equals(tag, requiredTag, StringComparison.OrdinalIgnoreCase)) return true;
        return false;
    }

    private static RoomConnector FindConnector(GameObject prefab, RoomConnector.ConnectionType type)
    {
        foreach (RoomConnector connector in prefab.GetComponentsInChildren<RoomConnector>(true))
            if (connector.type == type) return connector;
        return null;
    }
}
