using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;

public class StageWindowControl : MonoBehaviour
{
    // ================= Parent Object ================= //
    public GameObject bgPanel;
    // ================= Child Objects ================= //
    public Transform checkBoxes;
    public Color transparentColor;
    public GameObject balloon;
    // ================= Variables ================= //
    public List<bool> enabledBGRs = new List<bool>();
    public List<bool> checkedBGRs = new List<bool>();
    // ================= Default Func ================= //
    private void Awake()
    {
        CheckBGREnabled();
    }
    // ================= Func ================= //
    public void Activate()
    {
        bgPanel.SetActive(true);
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
    }
}

