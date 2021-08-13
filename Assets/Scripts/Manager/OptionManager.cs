using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// =================== Enum =================== //
public enum Resolution { FHD, HD }
public enum ScreenMode { NONE, FS, BLFS }
//
public class OptionManager : MonoBehaviour
{
    // =================== Children =================== //
    public GameObject option;
    //
    public List<CheckBoxControl> resolutionCB = new List<CheckBoxControl>();
    public List<CheckBoxControl> screenModeCB = new List<CheckBoxControl>();
    //
    // =================== Component =================== //
    // public GameObject assureWindow;
    // =================== Variable =================== //
    public Resolution curResolution;
    public ScreenMode curScreenMode;
    // public int checkedRS = 0, checkedSM = 0;
    // =================== Func - Default =================== //
    private void Awake()
    {
        option = transform.GetChild(0).gameObject;
        option.SetActive(true);
    }
    //
    private void Start()
    {
        SetOptionState();
        //
        option.SetActive(false);
    }
    private void Update()
    {
        if (TransitionControl.instance.isTransition == true)
            return;
        //
        string curSceneName = SceneManager.GetActiveScene().name;
        if (curSceneName == "1_FirstMenuScene")
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Backspace))
            {
                if(option.activeSelf == true)
                {
                    SoundManager.instance.PlayBTNClick();
                    option.SetActive(false);
                }
            }
        }
    }
    // =================== Func =================== //
    public void SetScreenAndCheckBoxFromSaveData()
    {
        // Set Screen Data
        curResolution = (Resolution)SceneControl.instance.saveData.resolution;
        curScreenMode = (ScreenMode)SceneControl.instance.saveData.screenMode;
        // Set CheckBox
        resolutionCB[(int)curResolution].JustCheck();
        int idx = (int)curScreenMode - 1;
        if (idx >= 0)
            screenModeCB[idx].JustCheck();
        //
        SetScreen();
    }
    //
    public void SetOptionState()
    {
        // Set Screen Data
        SetScreenAndCheckBoxFromSaveData();
        // Set Sound Data
        SoundManager.instance.SetSoundAndSliderFromSaveData();
    }
    // =================== Func - Button =================== //
    //
    public void BTN_CloseOptionWindow()
    {
        option.SetActive(false);
    }
    //
    public void ExitGame()
    { // Button Event
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBPLAYER
        Application.OpenURL("http://google.com");
#else
        Application.Quit(); //어플리케이션 종료
#endif
    }
    //
    public void ScreenChange()
    { // Called By Btn_CheckBox
        SetScreenStateByChecked();
        //
        SetScreen();
        // Save Option Data
        SceneControl.instance.SaveOption_Screen();
    }
    //
    public void SetScreen()
    {
        FullScreenMode fsMod = FullScreenMode.ExclusiveFullScreen;
        //
        switch (curScreenMode)
        {
            case ScreenMode.NONE:
                fsMod = FullScreenMode.Windowed;
                break;
            case ScreenMode.FS:
                fsMod = FullScreenMode.ExclusiveFullScreen;
                break;
            case ScreenMode.BLFS:
                fsMod = FullScreenMode.FullScreenWindow;
                break;
            default:
                break;
        }
        //
        int width = 0; int height = 0;
        switch (curResolution)
        {
            case Resolution.FHD:
                width = 1920; height = 1080;
                // Screen.SetResolution(1920, 1080, fsMod);
                break;
            case Resolution.HD:
                width = 1280; height = 720;
                // Screen.SetResolution(1280, 720, fsMod);
                break;
            default:
                break;
        }
        //
        Screen.SetResolution(width, height, fsMod);
    }
    //
    public void SetScreenStateByChecked()
    {
        curResolution = 0; curScreenMode = 0;
        //checkedRS = checkedSM = 0;
        //
        for (int i = 0; i <  resolutionCB.Count; ++i)
        {
            if (resolutionCB[i].isChecked == true)
            {
                curResolution = (Resolution)i;
                //checkedRS = i;
                break;
            }
        }
        //
        for (int i = 0; i <  screenModeCB.Count; ++i)
        {
            if (screenModeCB[i].isChecked == true)
            {
                curScreenMode = (ScreenMode)(i + 1);
                //checkedSM = i+1;
                break;
            }
        }
    }
}