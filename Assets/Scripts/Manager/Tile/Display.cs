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
    private Animator anim;
    private SpriteRenderer sr;
    //==================================//
    private readonly int hashPlayCount = Animator.StringToHash("PlayCount");
    private readonly int hashIsDanger = Animator.StringToHash("IsDanger");
    private readonly int hashMineType = Animator.StringToHash("MineType");
    private readonly int hashHit = Animator.StringToHash("Hit");
    private readonly int hashFlagHit = Animator.StringToHash("FlagHit");
    //==================================//
    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        //
        anim = GetComponent<Animator>();
        anim.enabled = false;
        //
        playCount = 5;
        anim.SetInteger(hashPlayCount, playCount);
    }
    //==================================//
    // Anim Frame Event
    public void SetItem()
    { // ShowItem : 0 frame
        ItemManager.instance.ShowItem(transform);
    }
    //==================================//
    public void Activate()
    {
        sr.enabled = true;
        Hit();
    }
    //
    public void SetNumber(int type)
    {
        sr.sprite = GameManager.instance.numIcons[type].texture;
    }
    public void SetDanger(bool danger, int type)
    {
        sr.sprite = GameManager.instance.mineIcons[type].texture;
        //
        anim.SetBool(hashIsDanger, danger);
        anim.SetFloat(hashMineType, type);
        //
        switch ((MineTypes)type)
        {
            case MineTypes.PULL:
            case MineTypes.PUSH:
            case MineTypes.GHOST:
                {
                    playCount = 5;
                    anim.SetInteger(hashPlayCount, playCount);
                }
                break;
            case MineTypes.THUNDER:
                {
                    playCount = 3;
                    anim.SetInteger(hashPlayCount, playCount);
                }
                break;
            case MineTypes.NARROWING:
            case MineTypes.CRASH:
            default:
                break;
        }
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
                GameManager.instance.mineIcons[(int)inner.mineType].texture;
            sr.color = Color.white;
            //
            anim.enabled = false;
            //
            MineController.instance.ActivateMine(inner);
        }
    }
}
