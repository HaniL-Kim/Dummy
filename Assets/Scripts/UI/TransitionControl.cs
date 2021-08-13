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
    public RectTransform left;
    private Vector3 leftOpen;
    private Vector3 leftClose;
    //
    public RectTransform right;
    private Vector3 rightOpen;
    private Vector3 rightClose;
    //
    public float tweenTime;
    //
    public bool isTransition = false;
    public bool isActivating = false;
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
        //
        isTransition = b;
    }
    //
    public void Door_Close()
    {
        SoundManager.instance.Play(SoundKey.DOOR_MOVE);
        //
        ActivateTransition();
        //
        left.DOAnchorPosX(leftClose.x, tweenTime).SetEase(Ease.InOutCubic).SetUpdate(true);;
        right.DOAnchorPosX(rightClose.x, tweenTime).SetEase(Ease.InOutCubic).SetUpdate(true);;
    }
    //
    public void Door_Open(string sceneName = "")
    {
        SoundManager.instance.Play(SoundKey.DOOR_MOVE);
        //
        TweenCallback CamSet;
        if(sceneName == "MainScene")
        {
            SceneControl.instance.cam.GetComponent<AudioListener>().enabled = false;
            //
            CamSet = () => { SceneControl.instance.UICamSet(false); };
            //
            Sequence open = DOTween.Sequence()
                .AppendInterval(0.5f)
                .Join(left.DOAnchorPosX(leftOpen.x, tweenTime).SetEase(Ease.InOutCubic))
                .Join(right.DOAnchorPosX(rightOpen.x, tweenTime).SetEase(Ease.InOutCubic))
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
                .Join(left.DOAnchorPosX(leftOpen.x, tweenTime).SetEase(Ease.InOutCubic))
                .Join(right.DOAnchorPosX(rightOpen.x, tweenTime).SetEase(Ease.InOutCubic))
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