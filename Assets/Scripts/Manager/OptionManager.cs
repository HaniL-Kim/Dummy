using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public GameObject assureWindow;
    // =================== Component =================== //
    public Resolution curResolution;
    public ScreenMode curScreenMode;
    // =================== Variable =================== //
    public int checkedRS = 0, checkedSM = 0;
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
    // =================== Func =================== //
    public void ResetOption()
    {

    }
    //
    private void SetScreenOption()
    {
        // Set Screen Data
        curResolution = (Resolution)SceneControl.instance.saveData.resolution;
        curScreenMode = (ScreenMode)SceneControl.instance.saveData.screenMode;
        // Set CheckBox
        resolutionCB[(int)curResolution].CheckInGroup();
        int idx = (int)curScreenMode - 1;
        if(idx >= 0)
            screenModeCB[idx].CheckInGroup();
    }
    //
    public void SetOptionState()
    {
        SetScreenOption();
        // Set Sound Data
        SoundManager.instance.SetSoundOption();
    }
    // =================== Func - Button =================== //
    private void ResetScreenModeCheckBox()
    {
        resolutionCB[(int)curResolution].CheckInGroup();
        //
        int iSM = (int)curScreenMode;
        if (iSM == 0)
        {
            foreach (CheckBoxControl cb in screenModeCB)
                cb.Check(false);
        }
        else
        {
            screenModeCB[iSM - 1].CheckInGroup();
        }
    }
    public void BTN_CloseOptionWindow()
    {
        ResetScreenModeCheckBox();
        //
        SoundManager.instance.SetSliderValue();
        SceneControl.instance.SetScreen();
        //
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
    public void BTN_Assure_Yes()
    {
        // Save Resolution & ScreenMode
        curResolution = (Resolution)checkedRS;
        curScreenMode = (ScreenMode)checkedSM;
        //
        SoundManager.instance.SetVolumeBySlider();
        SceneControl.instance.SetScreen((int)curResolution, (int)curScreenMode);
        //
        // Save Option Data
        SceneControl.instance.SaveOption((int)curResolution, (int)curScreenMode,
            SoundManager.instance.bgrVolume, SoundManager.instance.effVolume);
        //
        assureWindow.SetActive(false);
        //
        // Restart Program
        // ExitGame();
    }
    //
    public void BTN_Assure_No()
    {
        assureWindow.SetActive(false);
    }
    //
    public void BTN_Apply()
    {
        SetCheck();
        //
        SoundManager.instance.SetVolumeBySlider();
        //
        if ((checkedRS != (int)curResolution) || (checkedSM != (int)curScreenMode))
            assureWindow.SetActive(true);
        else
            option.SetActive(false);
    }
    //
    public void SetScreenByChecked()
    {
        SetCheck();
        //
        SceneControl.instance.SetScreen(checkedRS, checkedSM);
    }
    //
    public void SetCheck()
    {
        checkedRS = checkedSM = 0;
        //
        for(int i = 0; i <  resolutionCB.Count; ++i)
        {
            if (resolutionCB[i].isChecked == true)
                checkedRS = i;
        }
        //
        for(int i = 0; i <  screenModeCB.Count; ++i)
        {
            if (screenModeCB[i].isChecked == true)
                checkedSM = i+1;
        }
    }
}