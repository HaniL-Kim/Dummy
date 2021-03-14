using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;
using TMPro;
//
using MyUtilityNS;

public class StageWindowControl : MonoBehaviour
{
    // ================= Parent Object ================= //
    public GameObject bgPanel;
    // ================= StageBtn Objects ================= //
    public List<GameObject> stages = new List<GameObject>();
    // ================= Child Objects ================= //
    public Transform checkBoxes;
    public Color transparentColor;
    public MineInfoControl mineInfoControl;
    // ================= Texts ================= //
    public TextMeshProUGUI stageName;
    public TextMeshProUGUI clearFloor;
    public TextMeshProUGUI spdUpCD;
    public TextMeshProUGUI isClear;
    public TextMeshProUGUI kindOfMine;
    public TextMeshProUGUI description;
    // ================= Variables ================= //
    public List<bool> enabledBGRs = new List<bool>();
    public List<bool> checkedBGRs = new List<bool>();
    // ================= StageData ================= //
    public DicStageData stageData;
    public SaveData saveData;
    // ================= Default Func ================= //
    private void Awake()
    {
        bgPanel = transform.parent.gameObject;
        CheckBGREnabled();
        //
        saveData = new SaveData();
    }
    private void Start()
    {
        ReadStageTable();
    }
    // ================= Func ================= //
    private void SetStageBtns()
    {
        stages[0].transform.GetChild(0).GetComponentInChildren<StageButton>().Activate();
        //
        for(int i = 0; i < stages.Count; ++i)
        {
            int clearStageMax = saveData.stageClear[i];
            for (int j = 0; j < clearStageMax; ++j)
            {
                stages[i].transform.GetChild(j+1).GetComponentInChildren<StageButton>().Activate();
            }

        }

    }
    //
    public void Load()
    {
        saveData = MyUtility.LoadDataFromJson();
        SetStageBtns();
        Debug.Log("Load Complete");
    }
    //
    public void Save()
    {
        MyUtility.SaveDataToJson(saveData);
        Debug.Log("Save Complete");
    }
    //
    public void ReadStageTable()
    {
        stageData = MyUtility.ReadStageData();
    }
    //
    private void SetStageData(string diff, int stageNum)
    {
        string stageNameStr = stageNum == 0 ? "Infinite" : stageNum.ToString();
        stageName.text = stageNameStr + " Stage";
        //
        StageData sd = stageData[diff][stageNum];
        //
        int floorCount = sd.blocks * 10;
        int unit = sd.unit;
        int countKindOfMine = 0;
        //
        if (stageNum == 0)
            clearFloor.text = "Infinite";
        else
            clearFloor.text = floorCount.ToString() + "F";
        //
        spdUpCD.text = unit.ToString();
        // From Save Data
        int clearStage = saveData.stageClear[MyUtility.DiffToInt(diff)];
        if (stageNum <= clearStage)
            isClear.text = "YES";
        else
            isClear.text = "NO";
        //
        int[] mineCountsOfBlock = { sd.pull, sd.push, sd.ghost, sd.thunder, sd.narrow, sd.crash };

        for(int i = 0; i < 6; ++i)
        {
            if(mineCountsOfBlock[i] != 0)
            {
                mineInfoControl.mineIcons[i].SetActive(true);
                int mineCount = mineCountsOfBlock[i] * sd.blocks;
                mineInfoControl.mineCountTexts[i].text = "x" + mineCount.ToString();
                ++countKindOfMine;
            }
            else
                mineInfoControl.mineIcons[i].SetActive(false);
        }
        // "kind(s) of mine"
        string kom = (countKindOfMine == 1) ? " kind of mine" : " kinds of mine";
        kindOfMine.text = countKindOfMine.ToString() + kom;
        // "in 10 floor"
        description.gameObject.SetActive(stageNum == 0);
    }
    //
    public void Activate(string diff, int stageNum)
    {
        bgPanel.SetActive(true);
        SetStageData(diff, stageNum);
    }
    //
    public void CheckBGREnabled()
    {
        for(int i = 0; i < checkBoxes.childCount; ++i)
        {
            Image img = checkBoxes.GetChild(i).GetComponent<Image>();
            if (enabledBGRs[i] == false)
            {
                img.color = transparentColor;
                img.transform.GetComponent<Button>().interactable = false;
            }
            else
            {
                img.color = Color.white;
                img.transform.GetComponent<Button>().interactable = true;
            }
        }
    }
    // ================= Btn Event ================= //
    public void BTN_Close()
    {
        bgPanel.gameObject.SetActive(false);
    }
    //
    public void BTN_CheckBGR()
    {
        // Get current Obj idx
        GameObject curObj = EventSystem.current.currentSelectedGameObject;
        int idx = (int)Char.GetNumericValue(curObj.name[0]);
        //
        for (int i = 0; i < checkBoxes.childCount; ++i)
        {
            if(idx == i)
                checkedBGRs[idx] = !checkedBGRs[idx];
            else
                checkedBGRs[i] = false;
            //
            checkBoxes.GetChild(i).GetChild(0).gameObject.SetActive(checkedBGRs[i]);
        }
        //
        // Reset for Highlight self
        EventSystem.current.SetSelectedGameObject(null);
    }
}
// ================= Custom Editor ================= //
[CustomEditor(typeof(StageWindowControl))]
public class ActivateButton : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        //
        StageWindowControl swc = (StageWindowControl)target;
        //
        if (GUILayout.Button("CheckBGRState"))
            swc.CheckBGREnabled();
        if (GUILayout.Button("Save"))
            swc.Save();
        if (GUILayout.Button("Load"))
            swc.Load();
    }
} 