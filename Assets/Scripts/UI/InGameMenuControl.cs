using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameMenuControl : MonoBehaviour
{
    public GameObject inGameMenu;
    public GameObject assure;
    public bool isActive;
    // ================ Func : Default ================ //
    private void Awake()
    {
        inGameMenu = transform.GetChild(0).gameObject;
    }
    //
    private void Update()
    {
        if (TransitionControl.instance.isTransition == true)
            return;
        //
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Backspace))
        {
            string curSceneName = SceneManager.GetActiveScene().name;
            if (curSceneName == "MainScene")
            {
                if (inGameMenu.activeSelf == false)
                { // Open IGM
                    if (GameManager.instance.stageClear == true)
                        return;
                    //
                    SetIGMControl(true);
                    return;
                }
                else
                {
                    if (SceneControl.instance.option.activeSelf == true)
                    { // Close Option
                        SceneControl.instance.option.SetActive(false);
                        return;
                    }
                    if (assure.activeSelf == true)
                    { // Close Assure
                        SetAssure(false);
                        return;
                    }
                    // Close IGM
                    SetIGMControl(false);
                }
            }
        }

    }
    // ================ Func ================ //
    public void SetIGMControl(bool b)
    {
        SetAssure(false);
        //
        SceneControl.instance.UICamSet(b);
        //
        GameManager.instance.Pause(b);
        //
        inGameMenu.SetActive(b);
        //
        if(b == false)
        { // Reset
            SceneControl.instance.OpenOption(false);
            ButtonManager bm = FindObjectOfType<ButtonManager>(true);
            if (bm != null)
                bm.BTN_Back();
        }
    }
    // ================ Func : Btns ================ //
    public void BTN_Assure_Yes()
    {
        // ExitGame
        Time.timeScale = 1.0f;
        SceneControl.instance.StartSceneTransition("2_StageSelectScene");
        SetAssure(false);
        SetIGMControl(false);
        //
        SoundManager.instance.StopBGR();
    }
    //
    public void BTN_Assure_No()
    {
        SetAssure(false);
    }
    //
    public void BTN_CloseIGM()
    {
        SetIGMControl(false);
    }
    //
    public void BTN_Infor()
    {
        SceneControl.instance.StartInforScene();
    }
    //
    public void BTN_Option()
    {
        SceneControl.instance.OpenOption(true);
    }
    //
    public void BTN_Exit()
    {
        SetAssure(true);
    }
    //
    public void SetAssure(bool b)
    {
        assure.SetActive(b);
    }
}
