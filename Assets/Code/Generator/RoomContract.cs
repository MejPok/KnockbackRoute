using System;
using UnityEngine;

/// <summary>Ordered room requirements for a generated run. Empty tags mean any room.</summary>
[CreateAssetMenu(menuName = "Generation/Room Contract")]
public class RoomContract : ScriptableObject
{
    [Serializable]
    public class Step
    {
        [Tooltip("Optional room tag required at this position, for example button or exit.")]
        public string requiredTag;
        [Min(0)] public int minimumDifficulty;
        [Min(0)] public int maximumDifficulty = 99;
    }

    [Tooltip("Each entry applies once at its room index. Rooms after the last step use difficulty-based random selection.")]
    public Step[] steps;

    public int StepCount => steps != null ? steps.Length : 0;

    public Step GetStep(int roomIndex)
    {
        if (steps == null || steps.Length == 0) return null;
        return roomIndex < steps.Length ? steps[roomIndex] : null;
    }
}
