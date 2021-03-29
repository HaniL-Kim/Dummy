using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public enum Difficulty { NORMAL, HARD, IMPOSSIBLE }
//
public class ButtonManager : MonoBehaviour
{
    public const float cScreenWidth = 1920.0f;
    // ================= Child ================= //
    public List<RectTransform> stagePanels;
    public int currentPanelIdx = 0;
    //
    public Button leftArrow;
    public Button rightArrow;
    // ================= Enum ================= //
    // ================= Variables ================= //
    public float panelTweenTime;
    // ================= Default Func ================= //
    private void Start()
    {
        SetArrowInteractable();
    }
    // ================= StageSelectScene Button Func ================= //
    private void SetArrowInteractable()
    {
        if (stagePanels == null || leftArrow == null || rightArrow == null)
            return;
        //
        if (currentPanelIdx == 0)
        {
            leftArrow.interactable = false;
            rightArrow.interactable = true;
        }
        else if (currentPanelIdx == stagePanels.Count - 1)
        {
            leftArrow.interactable = true;
            rightArrow.interactable = false;
        }
        else
        {
            leftArrow.interactable = true;
            rightArrow.interactable = true;
        }
    }
    //
    public void SetPanel(Difficulty dif)
    {
        currentPanelIdx = (int)dif;
        MovePanel(false);
    }
    //
    public void MovePanel(bool tween = true)
    {
        float i = (float)currentPanelIdx * -1.0f;
        //
        foreach (RectTransform panel in stagePanels)
        {
            float targetPos = (i++) * cScreenWidth;
            if (tween == true)
                panel.DOAnchorPosX(targetPos, panelTweenTime);
            else
            {
                Vector2 temp = panel.anchoredPosition;
                temp.x = targetPos;
                panel.anchoredPosition = temp;
            }
        }
        //
        SetArrowInteractable();
    }
    //
    public void BTN_SetDifficulty_Left()
    {
        --currentPanelIdx;
        //
        if (SceneManager.GetActiveScene().buildIndex == 2)
            SceneControl.instance.currentDifficulty = (Difficulty)currentPanelIdx;
        //
        MovePanel();
    }
    //
    public void BTN_SetDifficulty_Right()
    {
        ++currentPanelIdx;
        //
        if (SceneManager.GetActiveScene().buildIndex == 2)
            SceneControl.instance.currentDifficulty = (Difficulty)currentPanelIdx;
        //
        MovePanel();
    }
    //
    public void BTN_BackToFirstMenuScene()
    {
        DOTween.KillAll();
        SceneManager.LoadScene("1_FirstMenuScene");
    }
    //
    public void BTN_Back()
    {
        DOTween.KillAll();
        //
        Camera.main.gameObject.SetActive(false);
        //
        int idx = SceneManager.GetActiveScene().buildIndex;
        SceneManager.UnloadSceneAsync(idx);
        //
        GameObject[] roots = SceneManager.GetSceneAt(0).GetRootGameObjects();
        Camera cm; Canvas cv;
        foreach (GameObject obj in roots)
        {
            if (obj.TryGetComponent<Camera>(out cm) == true)
                obj.SetActive(true);
            else if (obj.TryGetComponent<Canvas>(out cv) == true)
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
        StartCoroutine(Infor());
    }
    private IEnumerator Infor()
    {
        Camera.main.gameObject.SetActive(false);
        GameObject.Find("Canvas").GetComponent<Canvas>().enabled = false;
        //
        AsyncOperation op = SceneManager.LoadSceneAsync("3_InforScene", LoadSceneMode.Additive);
        op.allowSceneActivation = false;
        while(op.isDone == false)
        {
            yield return null;
            if(op.progress >= 0.9f)
            {
                if(op.isDone == false)
                {
                    op.allowSceneActivation = true;
                }
                else
                {
                    Scene s = SceneManager.GetSceneAt(1);
                    SceneManager.SetActiveScene(s);
                }
            }
        }
    }
    //
    public void BTN_Option()
    {
        GameObject optionWindow = GameObject.FindGameObjectWithTag("OptionWindow");
        optionWindow.transform.GetChild(0).gameObject.SetActive(true);
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
