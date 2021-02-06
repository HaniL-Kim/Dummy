using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MyUtility
{
    public static T[] ShuffleArray<T>(T[] array, int seed = 0)
    { // The Fisher-Yates Shuffle
        System.Random prng = new System.Random(seed);

        for (int i = 0; i < array.Length - 1; i++)
        {
            int randomIndex = prng.Next(i, array.Length);
            T tempItem = array[randomIndex];
            array[randomIndex] = array[i];
            array[i] = tempItem;
        }

        return array;
    }
    //
    public static void DebugLogArr<T>(T[] array)
    {
        string tmp = "";
        for (int i = 0; i < array.Length; i++)
            tmp += "i[" + i + "] : " + array[i] + ", ";
        Debug.Log(tmp);
    }
}
