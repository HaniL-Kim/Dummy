using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RotaryHeart.Lib.SerializableDictionary;
//===========================================//
namespace MyUtilityNS
{
    [System.Serializable]
    public class StringInt : SerializableDictionaryBase<string, int> { }

    [System.Serializable]
    public class StringFloat : SerializableDictionaryBase<string, float> { }

    [System.Serializable]
    public class StringString : SerializableDictionaryBase<string, string> { }

    [System.Serializable]
    public class StageData
    {
        public int blocks;
        public int unit;
        public int pull, push, ghost, thunder, narrow, crash;
    }

    [System.Serializable]
    public class IntStageData : SerializableDictionaryBase<int, StageData> { }

    [System.Serializable]
    public class DicStageData : SerializableDictionaryBase<string, IntStageData> { }
}
//
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
    //
    public static void DownHeight(Transform tf, float value)
    {
        Vector3 temp = tf.position;
        temp.y -= value;
        tf.position = temp;
    }
    //
    public static void SwapPos(Transform crash, Transform tile)
    {
        Vector3 crashPos = crash.localPosition;
        Vector3 tilePos = tile.localPosition;
        //
        crashPos.z = 0;
        tilePos.z = -1;
        //
        crash.localPosition = tilePos;
        tile.localPosition = crashPos;
    }
    //
    public static void SwapNameAt(Transform crash, Transform tile, int idx)
    {
        char charCrash = crash.name[idx];
        char charTile = tile.name[idx];
        {
            char[] arrCharCrash = crash.name.ToCharArray();
            arrCharCrash[idx] = charTile;
            crash.name = new string(arrCharCrash);
        }
        {
            char[] arrCharTile = tile.name.ToCharArray();
            arrCharTile[idx] = charCrash;
            tile.name = new string(arrCharTile);
        }
    }
}
