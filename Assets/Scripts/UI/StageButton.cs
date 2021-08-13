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
    public Difficulty diff;
    public int stageNum;
    //========== Sprites ==========//
    public Texture2D texture_Locked;
    public Texture2D texture_UnLocked;
    //public Sprite sprite_OnMouse;
    //========== Component ==========//
    private Button btn;
    //========== Child ==========//
    public RectTransform nameCard;
    public Image target;
    public Material targetMaterial;
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
    //========== Unlock Effect ==========//
    public GameObject unlockEffect;
    //========== Func : Default ==========//
    private void Awake()
    {
        btn = GetComponent<Button>();
        targetTF = target.GetComponent<RectTransform>();
        // Create & Set Material Instance
        targetMaterial = Instantiate(target.material);
        target.material = targetMaterial;
        //
        stageNum = (int)Char.GetNumericValue(MyUtility.GetLastChar(transform.parent.name));
        diffStr = transform.parent.parent.name;
        switch (diffStr)
        {
            case "NORMAL":
                diff = Difficulty.NORMAL;
                break;
            case "HARD":
                diff = Difficulty.HARD;
                break;
            default:
                break;
        }
        //
        //texture_InActive = SpriteUtility.GetSpriteTexture(sprite_InActive, false);
        //texture_Normal = SpriteUtility.GetSpriteTexture(sprite_Normal, false);
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
    public void Activate(bool tween = true)
    {
        if (btn.interactable == true)
            return;
        // Activate Sequance
        if(tween == true)
        {
            Tween tween_Rotate = targetTF.DORotateQuaternion(endRot, tweenTime_Activate).SetEase(Ease.InCubic);
            Tween tween_TextureToTarget = targetMaterial.DOFloat(1.0f, hashFactor, tweenTime_Activate).SetEase(Ease.InCubic);
            Tween tween_TextureToMain = targetMaterial.DOFloat(0.0f, hashFactor, tweenTime_Activate).SetEase(Ease.InCubic);
            //
            Sequence ActivateSequence = DOTween.Sequence()
            .Append(tween_Rotate)
            .Join(tween_TextureToTarget)
            .AppendCallback(() => { unlockEffect.SetActive(true); })
            .AppendCallback(() => { targetMaterial.SetTexture(hashDiffuse_1, texture_UnLocked); })
            .Append(tween_TextureToMain)
            .AppendCallback(() => { target.material = null; })
            .AppendCallback(() => { TransitionControl.instance.isActivating = false; })
            .AppendCallback(() => { btn.interactable = true; })
            ;
        }
        else
        {
            targetTF.rotation = endRot;
            targetMaterial.SetTexture(hashDiffuse_1, texture_UnLocked);
            target.material = null;
            //
            btn.interactable = true;
        }
        // Set nameCard Text
        char c = MyUtility.GetLastChar(transform.parent.name);
        string stageNum = "";
        if (c == '0')
        {
            stageNum = "Infinite";
            nameCard.GetComponentInChildren<TextMeshProUGUI>().text
                = stageNum;
        }
        else
        {
            stageNum = c.ToString();
            nameCard.GetComponentInChildren<TextMeshProUGUI>().text
                = "Stage " + stageNum;
        }
    }
    //
    public void DeActivate()
    {
        btn.interactable = false;
        //
        targetTF.rotation = beginRot;
        target.material = targetMaterial;
        targetMaterial.SetTexture(hashDiffuse_1, texture_Locked);
        //
        unlockEffect.SetActive(false);
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
