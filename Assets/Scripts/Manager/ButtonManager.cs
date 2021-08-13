using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public enum Difficulty { NORMAL, HARD, IMPOSSIBLE }
//
public class ButtonManager : MonoBehaviour
{
    // ================= Child ================= //
    public List<RectTransform> stagePanels;
    public UIArrowPanelControl upc;
    public StageWindowControl stageWindow;
    // ================= Default Func ================= //
    private void Start()
    {
        if (upc != null)
            upc.SetArrowInteractable();
    }
    //
    private void Update()
    {
        if (TransitionControl.instance.isTransition == true)
            return;
        if (TransitionControl.instance.isActivating == true)
            return;
        //
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Backspace))
        {
            string curSceneName = SceneManager.GetActiveScene().name;
            //
            if (curSceneName == "2_StageSelectScene")
            {
                SoundManager.instance.PlayBTNClick();
                //
                if (stageWindow.bgPanel.activeSelf == true)
                {
                    stageWindow.BTN_Close();
                    return;
                }
                else
                    BTN_BackToFirstMenuScene();
            }
            else if (curSceneName == "3_InforScene")
            {
                SoundManager.instance.PlayBTNClick();
                //
                BTN_Back();
            }
        }
    }
    // ================= StageSelectScene Button Func ================= //
    public void SetPanel(Difficulty dif)
    {
        upc.currentPanelIdx = (int)dif;
        upc.MovePanel(false);
    }
    //
    public void BTN_BackToFirstMenuScene()
    {
        DOTween.KillAll();
        SceneManager.LoadScene("1_FirstMenuScene");
    }
    //
    public void BTN_Back()
    { // Called Btn_Back of InforScene
        DOTween.KillAll();
        //
        int idx = SceneManager.GetActiveScene().buildIndex;
        SceneManager.UnloadSceneAsync(idx);
        //
        string sceneName = SceneManager.GetSceneAt(0).name;
        if (sceneName == "MainScene")
        {
            Canvas mainCanvas
                = GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<Canvas>();
            mainCanvas.worldCamera = Camera.main;
        }
        //GameObject[] roots = SceneManager.GetSceneAt(0).GetRootGameObjects();
        //Canvas cv;
        //foreach (GameObject obj in roots)
        //{
        //    if (obj.TryGetComponent<Canvas>(out cv) == true)
        //        obj.GetComponent<Canvas>().enabled = true;
        //}
    }
    // ================= FirstMenu Scene Button Func ================= //
    public void BTN_StageSelect()
    {
        SceneManager.LoadScene("2_StageSelectScene");
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
    //=========================================//
    public void BTN_Exit()
    { // Button Event
        SceneControl.instance.ExitGame();
    }
    //=========================================//
}
