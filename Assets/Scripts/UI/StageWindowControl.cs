using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;
//
using MyUtilityNS;
//
#if UNITY_EDITOR
using UnityEditor;
#endif

public class StageWindowControl : MonoBehaviour
{
    // ================= Parent Object ================= //
    public GameObject bgPanel;
    // ================= Child Objects ================= //
    public Transform checkBoxes;
    public Color transparentColor;
    public MineInfoControl mineInfoControl;
    // ================= Texts ================= //
    public TextMeshProUGUI stageName;
    public TextMeshProUGUI clearFloor;
    public TextMeshProUGUI spdUpCD;
    public TextMeshProUGUI clearTextValue;
    public TextMeshProUGUI clearTextName;
    //
    public TextMeshProUGUI kindOfMine;
    public TextMeshProUGUI description;
    // ================= Variables ================= //
    public List<bool> enabledBGR = new List<bool>();
    public List<bool> checkedBGR = new List<bool>();
    // ================= Default Func ================= //
    private void Awake()
    {
        bgPanel = transform.parent.gameObject;
        CheckBGREnabled();
    }
    //
    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Backspace))
    //    {
    //        BTN_Close();
    //    }
    //}
    // ================= Func ================= //
    private void SetStageData(string diff, int stageNum)
    {
        SceneControl.instance.currentStage = stageNum;
        //
        stageName.text = stageNum == 0 ? "Infinite" : stageNum.ToString() + " Stage";
        //
        StageData sd = SceneControl.instance.stageData[diff][stageNum];
        //
        int floorCount = sd.blocks * 10;
        int unit = sd.unit;
        int countKindOfMine = 0;
        //
        if (stageNum == 0)
            clearFloor.text = "-";
        else
            clearFloor.text = floorCount.ToString() + "F";
        //
        spdUpCD.text = unit.ToString();
        // Set Clear Text(Row-3)
        int stageInt = MyUtility.DiffToInt(diff);
        string bestRecord = SceneControl.instance.saveData.bestRecord[stageInt];
        //
        if (stageNum == 0)
        { // Infinite Stage
            clearTextName.text = "Best Record";
            clearTextValue.text = bestRecord + "F";
        }
        else
        { // 1 ~ 6 Stage
            clearTextName.text = "Clear";
            /* stageBtnState
                0 : locked / 1 : unlock effect / 2 : unlocked / 3 : clear */
            if (SceneControl.instance.saveData.stageBtnState[stageInt][stageNum] == 3)
                clearTextValue.text = "YES";
            else
                clearTextValue.text = "NO";
        }
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
        //
        SetStageData(diff, stageNum);
    }
    //
    public void CheckBGREnabled()
    {
        SceneControl sc = SceneControl.instance;
        ButtonManager bm = SceneControl.instance.bm;
        { // Set Enabled from saveData
            if (sc.saveData.stageBtnState[1][6] == 3)
            {// check hard stage 6 is cleared
                enabledBGR[1] = true;
                enabledBGR[2] = true;
            }
            {// check normal stage 6 is cleared
                if (sc.saveData.stageBtnState[0][6] == 3)
                    enabledBGR[1] = true;
            }
        }
        //
        // Set CheckBox Image & Interactable
        for (int i = 0; i < checkBoxes.childCount; ++i)
        {
            Image img = checkBoxes.GetChild(i).GetComponent<Image>();
            //
            if (enabledBGR[i] == false)
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
    public void BTN_Play()
    {
        // Transition Sequence
        SceneControl.instance.StartSceneTransition("MainScene");
        //Sequence ActivateSequence = DOTween.Sequence()
        //.AppendCallback(() => { TransitionControl.instance.Door_Close(); })
        //.AppendInterval(TransitionControl.instance.tweenTime)
        //.AppendCallback(() => { SceneControl.instance.StartMainScene(); })
        //;
    }
    //
    public void BTN_Close()
    {
        bgPanel.gameObject.SetActive(false);
    }
    //
    public void CheckBGR(int idx)
    {
        for (int i = 0; i < checkBoxes.childCount; ++i)
        {
            if (idx == i)
                checkedBGR[i] = true;
            else
                checkedBGR[i] = false;
            //
            checkBoxes.GetChild(i).GetChild(0).gameObject.SetActive(checkedBGR[i]);
        }
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
            {
                checkedBGR[idx] = !checkedBGR[idx];
                //
                if (checkedBGR[idx] == false)
                    SceneControl.instance.currentBGR = -1;
                else
                    SceneControl.instance.currentBGR = idx;
            }
            else
                checkedBGR[i] = false;
            //
            checkBoxes.GetChild(i).GetChild(0).gameObject.SetActive(checkedBGR[i]);
        }
        // Reset for Highlight self
        EventSystem.current.SetSelectedGameObject(null);
        // Save Current BGR Select
    }
    // ================= Custom Editor ================= //
#if UNITY_EDITOR
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
        }
    }
#endif
}