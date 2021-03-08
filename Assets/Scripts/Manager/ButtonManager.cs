using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ButtonManager : MonoBehaviour
{
    public const float cScreenWidth = 1920.0f;
    // ================= Child ================= //
    public List<RectTransform> stagePanels;
    public Button leftArrow;
    public Button rightArrow;
    // ================= Enum ================= //
    public enum Difficulty { NORMAL, HARD, IMPOSSIBLE }
    // ================= Variables ================= //
    public Difficulty currentDifficulty = Difficulty.NORMAL;
    public float difficultyTweenTime;
    // ================= Default Func ================= //
    private void Awake()
    {
        if(SceneManager.GetActiveScene().buildIndex == 1)
            SetArrowInteractable();
    }
    // ================= StageSelectScene Button Func ================= //
    private void SetArrowInteractable()
    {
        if (currentDifficulty == Difficulty.NORMAL)
            leftArrow.interactable = false;
        else if (currentDifficulty == Difficulty.IMPOSSIBLE)
            rightArrow.interactable = false;
        else
        {
            leftArrow.interactable = true;
            rightArrow.interactable = true;
        }
    }
    //
    private void MovePanel()
    {
        float i = (float)currentDifficulty * -1.0f;
        foreach (RectTransform panel in stagePanels)
        {
            float targetPos = (i++) * cScreenWidth;
            panel.DOAnchorPosX(targetPos, difficultyTweenTime);
        }
        //
        SetArrowInteractable();
    }
    //
    public void BTN_SetDifficulty_Left()
    {
        --currentDifficulty;
        MovePanel();
    }
    //
    public void BTN_SetDifficulty_Right()
    {
        ++currentDifficulty;
        MovePanel();
    }
    //
    public void BTN_BackToFirstMenuScene()
    {
        DOTween.KillAll();
        SceneManager.LoadScene(0);
    }
    // ================= FirstMenu Scene Button Func ================= //
    public void BTN_StageSelect()
    {
        DOTween.KillAll();
        SceneManager.LoadScene(1);
    }
    //
    public void BTN_Infor()
    {
        Debug.Log("Infor");
    }
    //
    public void BTN_Option()
    {
        Debug.Log("Option");
    }
    //
    public void BTN_Credit()
    {
        Debug.Log("Credit");
    }
    //
    public void BTN_Exit()
    { // Button Event
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBPLAYER
        Application.OpenURL("http://google.com");
#else
        Application.Quit(); //어플리케이션 종료
#endif
    }
    //=========================================//
}
