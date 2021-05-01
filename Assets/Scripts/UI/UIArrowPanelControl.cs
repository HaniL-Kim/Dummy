using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class UIArrowPanelControl : MonoBehaviour
{
    public const float cScreenWidth = 1920.0f;
    // ================= Parent ================= //
    public ButtonManager bm;
    // ================= Child ================= //
    public Button leftArrow;
    public Button rightArrow;
    public TextMeshProUGUI panelCountTMP;
    // ================= Variable ================= //
    public float panelTweenTime;
    // ================= For check ================= //
    public int currentPanelIdx = 0;
    // ================= Default Func ================= //
    private void Awake()
    {
        bm = transform.parent.GetComponent<ButtonManager>();
        SetPanelCount();
    }
    // ================= Func ================= //
    private void SetPanelCount()
    {
        string s = (currentPanelIdx + 1).ToString() + " / " + bm.stagePanels.Count.ToString();
        panelCountTMP.text = s;
    }
    //
    public void SetArrowInteractable()
    {
        if (bm.stagePanels == null || leftArrow == null || rightArrow == null)
            return;
        //
        if (currentPanelIdx == 0)
        {
            leftArrow.interactable = false;
            rightArrow.interactable = true;
        }
        else if (currentPanelIdx == bm.stagePanels.Count - 1)
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
    public void BTN_Arrow_Left()
    {
        --currentPanelIdx;
        //
        if (SceneManager.GetActiveScene().buildIndex == 2)
            SceneControl.instance.currentDifficulty = (Difficulty)currentPanelIdx;
        //
        MovePanel(true);
    }
    //
    public void BTN_Arrow_Right()
    {
        ++currentPanelIdx;
        //
        if (SceneManager.GetActiveScene().buildIndex == 2)
            SceneControl.instance.currentDifficulty = (Difficulty)currentPanelIdx;
        //
        MovePanel(true);
    }
    //
    public void MovePanel(bool tween = true)
    {
        float i = (float)currentPanelIdx * -1.0f;
        //
        foreach (RectTransform panel in bm.stagePanels)
        {
            float targetPos = (i++) * cScreenWidth;
            if (tween == true)
                panel.DOAnchorPosX(targetPos, panelTweenTime).SetUpdate(true);
            else
            {
                Vector2 temp = panel.anchoredPosition;
                temp.x = targetPos;
                panel.anchoredPosition = temp;
            }
        }
        //
        SetPanelCount();
        //
        SetArrowInteractable();
    }
}
