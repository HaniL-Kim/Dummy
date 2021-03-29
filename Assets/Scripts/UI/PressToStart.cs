using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class PressToStart : MonoBehaviour
{
    public TextMeshProUGUI tmp;
    //==========================================//
    private void Awake()
    {
        tmp = GetComponent<TextMeshProUGUI>();
        BlinkText();
    }
    private void Update()
    {
        if(Input.anyKeyDown)
        {
            SceneManager.LoadScene(1);
        }
    }
    //==========================================//
    public void BlinkText()
    {
        tmp.DOColor(Color.black, 1.0f).SetLoops(-1, LoopType.Yoyo);
    }
    //==========================================//
}
