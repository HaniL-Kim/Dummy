using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;


public class CreditControl : MonoBehaviour
{
    public GameObject credit;
    public TextMeshProUGUI text;
    public Vector3 creditStart, creditEnd;
    public float rollTime = 10.0f;
    // =================== DEFAULT =================== //
    private void OnEnable()
    {
        text.rectTransform.anchoredPosition = creditStart;
        creditTextRoll();
    }
    // =================== BTN =================== //
    public void BTN_CloseCredit()
    {
        if(credit == null)
            credit = transform.parent.gameObject;
        //
        DOTween.KillAll();
        //
        credit.SetActive(false);
    }
    //
    public void BTN_OpenCredit()
    {
        if(credit == null)
            credit = transform.parent.gameObject;
        //
        credit.SetActive(true);
    }
    // =================== Func =================== //
    public void creditTextRoll()
    {
        text.rectTransform.DOAnchorPosY(creditEnd.y, rollTime);
    }

}
