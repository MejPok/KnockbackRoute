using UnityEngine;
using System.Collections.Generic;

public class Shuffler
{
    public static void Shuffle(List<RoomDefinition> items, System.Random random)
    {
        for (int i = items.Count - 1; i > 0; i--)
        {
            int swapIndex = random.Next(i + 1);
            (items[i], items[swapIndex]) = (items[swapIndex], items[i]);
        }
    }   

    public static void Shuffle(List<RoomContract> items, System.Random random)
    {
        for (int i = items.Count - 1; i > 0; i--)
        {
            int swapIndex = random.Next(i + 1);
            (items[i], items[swapIndex]) = (items[swapIndex], items[i]);
        }   
    }   

    public static void Shuffle(RoomContract[] items, System.Random random)
    {
        for (int i = items.Length - 1; i > 0; i--)
        {
            int swapIndex = random.Next(i + 1);
            (items[i], items[swapIndex]) = (items[swapIndex], items[i]);
        }   
    }   
}
