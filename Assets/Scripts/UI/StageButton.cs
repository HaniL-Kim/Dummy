using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using MyUtilityNS;

public class StageButton : MonoBehaviour
{
    [SerializeField]
    private StageWindowControl swc;
    //========== ID ==========//
    public string diffStr;
    public int stageNum;
    //========== Sprites ==========//
    public Sprite sprite_InActive;
    public Sprite sprite_OnMouse;
    public Sprite sprite_Normal;
    //========== Component ==========//
    private Button btn;
    //========== Child ==========//
    public RectTransform nameCard;
    public Image target;
    public RectTransform targetTF;
    //========== Activate Btn ==========//
    public float tweenTime_Activate;
    public Quaternion beginRot;
    public Quaternion endRot;
    //========== Popup NameCard ==========//
    public float tweenTime_Popup;
    public float startPos;
    public float endPos;
    //========== Texture Blending ==========//
    private readonly int hashDiffuse_1 = Shader.PropertyToID("_Diffuse_1");
    private readonly int hashFactor = Shader.PropertyToID("_TransitionFactor");
    //========== Func : Default ==========//
    private void Awake()
    {
        btn = GetComponent<Button>();
        targetTF = target.GetComponent<RectTransform>();
        // Create & Set Material Instance
        Material mat = Instantiate(target.material);
        target.material = mat;
        //
        stageNum = (int)Char.GetNumericValue(MyUtility.GetLastChar(transform.parent.name));
        diffStr = transform.parent.parent.name;
    }
    private void Start()
    {
        swc = FindObjectOfType<StageWindowControl>(true);
    }
    //========== Func  ==========//
    public void ActivateStageWindow()
    {
        swc.Activate(diffStr, stageNum);
    }
    //========== Func : Call From DataManager ==========//
    public void Activate()
    {
        if (btn.interactable == true)
            return;
        //
        btn.interactable = true;
        // Activate Sequance
        {
            Tween tween_Rotate = targetTF.DORotateQuaternion(endRot, tweenTime_Activate).SetEase(Ease.InCubic);
            Tween tween_TextureToTarget = target.material.DOFloat(1.0f, hashFactor, tweenTime_Activate).SetEase(Ease.InCubic);
            Tween tween_TextureToMain = target.material.DOFloat(0.0f, hashFactor, tweenTime_Activate).SetEase(Ease.InCubic);
            //
            Sequence ActivateSequence = DOTween.Sequence()
            .Append(tween_Rotate)
            .Join(tween_TextureToTarget)
            .AppendCallback(() => { target.material.SetTexture(hashDiffuse_1, sprite_Normal.texture); })
            .Append(tween_TextureToMain)
            .AppendCallback(() => { target.material = null; })
            ;
        }
        // To Fix : add number
        nameCard.GetComponentInChildren<TextMeshProUGUI>().text = "Stage";
    }
    //
    public void DeActivate()
    {
        btn.interactable = false;
        // DeActivate Sequance
        {
            Tween tween_Rotate = targetTF.DORotateQuaternion(beginRot, tweenTime_Activate).SetEase(Ease.InCubic);
            Tween tween_Texture = target.material.DOFloat(0.0f, hashFactor, tweenTime_Activate).SetEase(Ease.InCubic);
            Tween tween_TextureToMain = target.material.DOFloat(0.0f, hashFactor, tweenTime_Activate).SetEase(Ease.InCubic);
            //
            Sequence DeActivateSequence = DOTween.Sequence()
            .SetAutoKill(false)
            .Append(tween_Rotate)
            .Join(tween_Texture)
            .AppendCallback(() =>
            {
                target.material.SetTexture(hashDiffuse_1, sprite_InActive.texture);
            })
            .Append(tween_TextureToMain)
            ;
        }
        //DeActivateSequence.Restart();
    }
    //========== Func : EventTrigger ==========//
    public void BTN_OpenNameCard()
    {
        nameCard.DOLocalMoveX(endPos, tweenTime_Popup).SetEase(Ease.InOutCubic);
    }
    //
    public void BTN_CloseNameCard()
    {
        nameCard.DOLocalMoveX(startPos, tweenTime_Popup).SetEase(Ease.InOutCubic);
    }
}
