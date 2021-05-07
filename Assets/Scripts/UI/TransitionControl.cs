using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
//
#if UNITY_EDITOR
using UnityEditor;
#endif

public class TransitionControl : MonoBehaviour
{
    public static TransitionControl instance;
    //
    public GameObject transition;
    //
    //public RectTransform up;
    //private Vector3 upOpen;
    //private Vector3 upClose;
    //
    public RectTransform left;
    private Vector3 leftOpen;
    private Vector3 leftClose;
    //
    public RectTransform right;
    private Vector3 rightOpen;
    private Vector3 rightClose;
    //
    public float tweenTime;
    // ================ Func : Default ================ //
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //
            InitTransition();
        }
        else
            Destroy(this.gameObject);
    }
    // ================ Func  ================ //
    private void InitTransition()
    {
        // Transition
        transition = transform.GetChild(0).gameObject;
        // Door_Left
        left = transition.transform.GetChild(0).GetComponent<RectTransform>();
        leftOpen = left.anchoredPosition;
        leftClose = Vector3.zero;
        // Door_Right
        right = transition.transform.GetChild(1).GetComponent<RectTransform>();
        rightOpen = right.anchoredPosition;
        rightClose = Vector3.zero;
        //
        transition.SetActive(false);
    }
    public void ActivateTransition(bool b = true)
    {
        SceneControl.instance.UICamSet(true);
        //
        transition.SetActive(b);
    }
    //
    public void Door_Close()
    {
        ActivateTransition();
        //
        //up.DOAnchorPosY(upClose.y, tweenTime).SetEase(Ease.InOutCubic).SetUpdate(true);
        //
        left.DOAnchorPosX(leftClose.x, tweenTime).SetEase(Ease.InOutCubic).SetUpdate(true);;
        right.DOAnchorPosX(rightClose.x, tweenTime).SetEase(Ease.InOutCubic).SetUpdate(true);;
    }
    //
    public void Door_Open(string sceneName = "")
    {
        TweenCallback CamSet;
        if(sceneName == "MainScene")
        {
            SceneControl.instance.cam.GetComponent<AudioListener>().enabled = false;
            //
            CamSet = () => { SceneControl.instance.UICamSet(false); };
            //
            Sequence open = DOTween.Sequence()
                .AppendInterval(1.0f)
                //.Append(up.DOAnchorPosY(upOpen.y, tweenTime).SetEase(Ease.InOutCubic))
                .Join(left.DOAnchorPosX(leftOpen.x, tweenTime).SetEase(Ease.InOutCubic))
                .Join(right.DOAnchorPosX(rightOpen.x, tweenTime).SetEase(Ease.InOutCubic))
                //.AppendInterval(tweenTime * 2.0f)
                .AppendCallback(() => { ActivateTransition(false); })
                .AppendCallback(CamSet)
                .SetUpdate(true)
                ;
        }
        else
        {
            SceneControl.instance.cam.GetComponent<AudioListener>().enabled = true;
            //
            CamSet = () => { SceneControl.instance.UICamSet(true); };
            //
            Sequence open = DOTween.Sequence()
                .AppendCallback(CamSet)
                //.Append(up.DOAnchorPosY(upOpen.y, tweenTime).SetEase(Ease.InOutCubic))
                .Join(left.DOAnchorPosX(leftOpen.x, tweenTime).SetEase(Ease.InOutCubic))
                .Join(right.DOAnchorPosX(rightOpen.x, tweenTime).SetEase(Ease.InOutCubic))
                //.AppendInterval(tweenTime * 2.0f)
                .AppendCallback(() => { ActivateTransition(false); })
                .SetUpdate(true)
                ;
        }
    }
}
// ================= Custom Editor ================= //
#if UNITY_EDITOR
[CustomEditor(typeof(TransitionControl))]
public class ActivateButton : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        //
        TransitionControl control = (TransitionControl)target;
        //
        if (GUILayout.Button("Door_Open"))
            control.Door_Open();
        if (GUILayout.Button("Door_Close"))
            control.Door_Close();
    }
}
#endif