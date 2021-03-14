using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class MineInfoControl : MonoBehaviour
{
    const float cIconWidth = 192.0f;
    public RectTransform contentRTF;
    private Vector2 pivotDefault = new Vector2(0.5f, 0.5f);
    private Vector2 pivotLeft = new Vector2(0.0f, 0.5f);
    //
    public List<GameObject> mineIcons = new List<GameObject>();
    public List<TextMeshProUGUI> mineCountTexts;

    private int mineCount;
    //
    public Button leftBtn;
    public Button rightBtn;
    public int currentIconIndex;
    //===========================================//
    private void Awake()
    {
        mineCountTexts = new List<TextMeshProUGUI>();
        for (int i = 0; i < mineIcons.Count; ++i)
            mineCountTexts.Add( mineIcons[i].GetComponentInChildren<TextMeshProUGUI>(true) );
    }
    //
    private void Start()
    {
        //CheckMineCount();
        //ContentPivotControl();
    }
    //
    private void Update()
    {
        CheckMineCount();
        ContentPivotControl();
        CheckContentArrowBtn();
    }
    //===========================================//
    private void CheckContentArrowBtn()
    {
        if(mineCount <= 3)
        {
            leftBtn.gameObject.SetActive(false);
            rightBtn.gameObject.SetActive(false);
            return;
        }
        //
        if (currentIconIndex == 0)
        {
            leftBtn.gameObject.SetActive(false);
            rightBtn.gameObject.SetActive(true);
        }
        else if (currentIconIndex == mineCount - 3)
        {
            leftBtn.gameObject.SetActive(true);
            rightBtn.gameObject.SetActive(false);
        }
        else
        {
            leftBtn.gameObject.SetActive(true);
            rightBtn.gameObject.SetActive(true);
        }
    }
    //
    private void CheckMineCount()
    {
        mineCount = 0;
        //
        for (int i = 0; i < mineIcons.Count; ++i)
            if (mineIcons[i].activeSelf == true)
                mineCount++;
    }
    //
    private void ContentPivotControl()
    { // Call Once on Awkae
        if (contentRTF == null)
            return;
        //
        if (mineCount >= 3)
            contentRTF.pivot = pivotLeft;
        else // if (mineCount < 3)
            contentRTF.pivot = pivotDefault;
    }
    //===========================================//
    private void TweenContentPos()
    {
        float destPos = -currentIconIndex * cIconWidth;
        contentRTF.DOAnchorPosX(destPos, 1.0f).SetEase(Ease.InOutCubic);
    }
    //
    public void Btn_MoveContentLeft()
    {
        --currentIconIndex;
        TweenContentPos();
    }
    //
    public void Btn_MoveContentRight()
    {
        ++currentIconIndex;
        TweenContentPos();
    }
}
