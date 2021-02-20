using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MyUtility
{
    public static List<T> ShuffleList<T>(List<T> list)
    {
        int seed = Random.Range(0, 1000);
        //
        System.Random prng = new System.Random(seed);

        for (int i = 0; i < list.Count - 1; i++)
        {
            int randomIndex = prng.Next(i, list.Count);
            T tempItem = list[randomIndex];
            list[randomIndex] = list[i];
            list[i] = tempItem;
        }

        return list;
    }
    //
    public static T[] ShuffleArray<T>(T[] array)
    { // The Fisher-Yates Shuffle
        int seed = Random.Range(0, 1000);
        //
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
    public static void DebugLogList<T>(List<T> list)
    {
        string tmp = "";
        //
        for (int i = 0; i < list.Count; i++)
        {
            tmp += "i[" + i + "] : " + list[i] + ", ";
            if (i % 10 == 9)
                tmp += "\n";
        }
        //
        Debug.Log(tmp);
    }
    //
    public static void DebugLogArr<T>(T[] array)
    {
        string tmp = "";
        for (int i = 0; i < array.Length; i++)
            tmp += "i[" + i + "] : " + array[i] + ", ";
        Debug.Log(tmp);
    }
    //
    public static bool IsNull<T>(T value, string name)
    {
        bool result = false;
        //
        if (value == null)
        {
            Debug.LogError(name + " : access null");
            result = true;
        }

        return result;
    }
}
