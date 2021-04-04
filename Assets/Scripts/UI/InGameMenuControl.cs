using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    // ================ Func ================ //
    public void Set(bool b)
    {
        UIManager.instance.SetPause(b);
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
        //SceneControl.instance.ExitGame();
        Time.timeScale = 1.0f;
        SceneControl.instance.StartSceneTransition("2_StageSelectScene");
        SetAssure(false);
        Set(false);
        //
    }
    //
    public void BTN_Assure_No()
    {
        SetAssure(false);
    }
    //
    public void BTN_CloseIGM()
    {
        Set(false);
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
