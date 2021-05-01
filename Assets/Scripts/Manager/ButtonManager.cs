using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//using UnityEngine.UI;
//using UnityEngine.EventSystems;
using DG.Tweening;

public enum Difficulty { NORMAL, HARD, IMPOSSIBLE }
//
public class ButtonManager : MonoBehaviour
{
    // ================= Child ================= //
    public List<RectTransform> stagePanels;
    public UIArrowPanelControl upc;
    // ================= Default Func ================= //
    private void Start()
    {
        if(upc != null)
            upc.SetArrowInteractable();
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
        GameObject[] roots = SceneManager.GetSceneAt(0).GetRootGameObjects();
        Canvas cv;
        foreach (GameObject obj in roots)
        {
            if (obj.TryGetComponent<Canvas>(out cv) == true)
                obj.GetComponent<Canvas>().enabled = true;
        }
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
