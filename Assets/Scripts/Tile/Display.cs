using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Display : MonoBehaviour
{
    //==================================//
    // Parent Obejct Script
    public Inner inner;
    //==================================//
    public int playCount;
    //==================================//
    public bool flagHit = false;
    //==================================//
    int type = 0;
    bool danger = false;
    //==================================//
    public Animator anim;
    public SpriteRenderer sr;
    //==================================//
    private readonly int hashPlayCount = Animator.StringToHash("PlayCount");
    private readonly int hashIsDanger = Animator.StringToHash("IsDanger");
    private readonly int hashMineType = Animator.StringToHash("MineType");
    private readonly int hashHit = Animator.StringToHash("Hit");
    private readonly int hashFlagHit = Animator.StringToHash("FlagHit");
    //==================================//
    private void OnEnable()
    {
        anim.SetBool(hashIsDanger, danger);
        anim.SetFloat(hashMineType, type);
        anim.SetInteger(hashPlayCount, playCount);
    }
    //==================================//
    // Anim Frame Event
    public void SetDisplayNumber()
    { // ShowItem : end frame
        anim.enabled = false;
        SetNumber(inner.dangerCount);
    }
    // Anim Frame Event
    public void SetItem()
    { // ShowItem : 0 frame
        ItemManager.instance.ShowItem(transform);
    }
    //==================================//
    public void ResetDisplay()
    {
        flagHit = false;
        SetDanger(danger = false, type = -1);
        anim.enabled = false;
        sr.enabled = false;
        //
        foreach (var item in GetComponentsInChildren<Item>())
            item.ResetItem();

    }
    //==================================//
    public void Activate()
    {
        sr.enabled = true;
        Hit();
    }
    //
    public void SetNumber(int _value)
    {
        sr.sprite = MineController.instance.numIcons[_value].texture;
    }
    public void SetDanger(bool _danger, int _type)
    {
        danger = _danger;
        type = _type;
        //
        sr.sprite = MineController.instance.mineIcons[type + 1].texture;
        //
        anim.SetBool(hashIsDanger, danger);
        anim.SetFloat(hashMineType, (float)type);
        //
        switch ((MineTypes)type)
        {
            case MineTypes.NONE:
                break;
            case MineTypes.PULL:
            case MineTypes.PUSH:
                playCount = 5;
                break;
            case MineTypes.GHOST:
            case MineTypes.THUNDER:
            case MineTypes.NARROWING:
            case MineTypes.CRASH:
                playCount = 4;
                break;
        }
        anim.SetInteger(hashPlayCount, playCount);
    }
    //
    public void Hit()
    {
        if (inner.isDanger)
        {
            anim.enabled = true;
            if (flagHit)
                anim.SetTrigger(hashFlagHit);
            else
                anim.SetTrigger(hashHit);
        }
    }
    //==================================//
    public void SubPlayCount()
    { // Anim Event
        --playCount;
        anim.SetInteger(hashPlayCount, playCount);
        //
        if (playCount == 0)
        {
            sr.sprite =
                MineController.instance.mineIcons[(int)inner.mineType + 1].texture;
            // debug
            sr.color = Color.white;
            //
            anim.enabled = false;
            //
            MineController.instance.ActivateMine(inner);
        }
    }
}
