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
    private Color originalColor;
    //==========================================//
    private void Awake()
    {
        tmp = GetComponent<TextMeshProUGUI>();
        originalColor = tmp.color;
    }
    //
    private void Update()
    {
        if (TransitionControl.instance.isTransition == true)
            return;
        //
        if (Input.anyKeyDown)
        {
            string curSceneName = SceneManager.GetActiveScene().name;
            //
            if (curSceneName == "0_LogoScene")
                SceneManager.LoadScene(1);
            else if (curSceneName == "MainScene")
            {
                if (GameManager.instance.stageClear == true)
                    StartCoroutine(SceneControl.instance.ClearSequence());
            }
            else
                return;
        }
    }
    //
    private void OnEnable()
    {
        BlinkText();
    }
    //
    private void OnDisable()
    {
        DOTween.Kill("BlinkText");
    }
    //==========================================//
    public void BlinkText()
    {
        tmp.color = originalColor;
        //
        tmp.DOColor(Color.black, 1.0f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetId("BlinkText")
            .SetUpdate(true)
            ;
    }
    //==========================================//
}
