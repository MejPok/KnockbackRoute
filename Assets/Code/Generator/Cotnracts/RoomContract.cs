using System;
using UnityEngine;
using UnityEngine.Rendering;

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

        [Min(1)] public int Count;
        [Min(1)] public int minCount = 1;
        [Min(1)] public int maxCount = 1;
        public bool enableRandomCount;
        
        public int getRealCount()
        {
            int tempCount;
            if (enableRandomCount)
            {
                tempCount= new System.Random().Next(minCount, maxCount);
            } else
            {
                tempCount = minCount;
            }

            return tempCount;
        } 



    }

    [Tooltip("Each entry applies once at its room index. Rooms after the last step use difficulty-based random selection.")]
    public Step[] steps;

    
    public int GetAverageContractDifficulty()
    {
        int allDifficulty = 0;
        foreach(var step in steps)
        {
            int averageStepDifficulty = (step.minimumDifficulty + step.maximumDifficulty) / 2;
            allDifficulty += averageStepDifficulty * step.getRealCount();
        }
        return allDifficulty;
    } 

    public int ExpectedDifficulty;

    public int StepCount => steps != null ? steps.Length : 0;

    public Step GetStep(int roomIndex)
    {
        if (steps == null || steps.Length == 0) return null;
        return roomIndex < steps.Length ? steps[roomIndex] : null;
    }
}
