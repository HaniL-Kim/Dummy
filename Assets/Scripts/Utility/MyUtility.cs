﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RotaryHeart.Lib.SerializableDictionary;
using MyUtilityNS;
using LitJson;
using System;
using System.IO;
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
//================================================//
[Serializable]
public class SaveData
{
    //=========== Variables ===========//
    public string id;
    public List<int> stageClear;
    //
    public double bgrVolume;
    public double effVolume;
    //
    public int resolution;
    public int screenMode;
    //=========== ctor ===========//
    // (Default)
    public SaveData()
    {
        id = "SaveData";
        //
        stageClear = new List<int>();
        stageClear.Add(0); // NORMAL
        stageClear.Add(0); // HARD
        stageClear.Add(0); // IMPOSSIBLE
        //
        bgrVolume = 0.5;
        effVolume = 0.5;
        //
        resolution = 0;
        screenMode = 0;
    }
    // from Jason
    public SaveData(JsonData jd)
    {
        id = jd["id"].ToString();
        //
        stageClear = new List<int>();
        for(int i = 0; i < jd["stageClear"].Count; ++i)
        {
            stageClear.Add(int.Parse(jd["stageClear"][i].ToString()));
        }
        //
        bgrVolume = double.Parse(jd["bgrVolume"].ToString());
        effVolume = double.Parse(jd["effVolume"].ToString());
        //
        resolution = int.Parse(jd["resolution"].ToString());
        screenMode = int.Parse(jd["screenMode"].ToString());
    }
}
//================================================//
public static class MyUtility
{
    public static string DiffToStr(Difficulty diff)
    {
        string result = "";
        //
        switch (diff)
        {
            case Difficulty.NORMAL:
                result = "NORMAL";
                break;
            case Difficulty.HARD:
                result = "HARD";
                break;
            case Difficulty.IMPOSSIBLE:
                result = "IMPOSSIBLE";
                break;
            default:
                break;
        }
        //
        return result;
    }
    //
    public static int DiffToInt(string s)
    {
        int result = 0;
        switch (s)
        {
            case "NORMAL":
                result = 0;
                break;
            case "HARD":
                result = 1;
                break;
            case "IMPOSSIBLE":
                result = 2;
                break;
            default:
                break;
        }
        return result;
    }

    public static char GetLastChar(string s)
    {
        char c = s[s.Length - 1];
        return c;
    }
    //
    public static void SaveDataToJson(SaveData sd)
    {
        JsonData jd = JsonMapper.ToJson(sd);
        //
        DirectoryInfo dir = Directory.CreateDirectory(Application.dataPath + "/SaveData");
        string savePath = Application.dataPath + "/SaveData/SaveData.json";
        //
        File.WriteAllText(savePath, jd.ToString());
    }
    //
    public static SaveData LoadDataFromJson()
    {
        string savePath = Application.dataPath + "/SaveData/SaveData.json";
        //
        string jsonString = "";
        // Initial SaveData
        if (File.Exists(savePath) == false)
            SaveDataToJson(new SaveData());
        //
        jsonString = File.ReadAllText(savePath);
        //
        JsonData jd = JsonMapper.ToObject(jsonString);
        //
        SaveData sd = new SaveData(jd);
        return sd;
    }
    //
    public static DicStageData ReadStageData()
    {
        DicStageData stageData = new DicStageData();
        List<StringString> ss = CSVReader.Read("MapData/StageData");
        //
        string[] diffs = { "NORMAL", "HARD", "IMPOSSIBLE" };
        for (int i = 0; i < diffs.Length; ++i)
            stageData.Add(diffs[i], new IntStageData());
        //
        foreach (var data in ss)
        {
            StageData sd = new StageData();
            //
            string difficulty = data["Difficulty"];
            int stage = int.Parse(data["Stage"]);
            //
            sd.blocks = int.Parse(data["Blocks"]);
            sd.unit = int.Parse(data["Unit"]);
            //
            sd.pull = int.Parse(data["Pull"]);
            sd.push = int.Parse(data["Push"]);
            sd.ghost = int.Parse(data["Ghost"]);
            sd.thunder = int.Parse(data["Thunder"]);
            sd.narrow = int.Parse(data["Narrow"]);
            sd.crash = int.Parse(data["Crash"]);
            //
            stageData[difficulty][stage] = sd;
        }
        //
        return stageData;
    }
    //
    public static List<T> ShuffleList<T>(List<T> list)
    {
        int seed = UnityEngine.Random.Range(0, 1000);
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
        int seed = UnityEngine.Random.Range(0, 1000);
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
