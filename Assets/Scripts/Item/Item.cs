﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum ItemType { RESOURCE, AJPACK, SHIELD, SLOW, COUNT }
    //===============================================//
    [SerializeField]
    private ItemType type;
    //===============================================//
    private Animator anim;
    //
    public int resourceItemValue = 50;
    //===============================================//
    private readonly int hashObtain = Animator.StringToHash("Obtain");
    private readonly int hashShowItem = Animator.StringToHash("ShowItem");
    private readonly int hashItemType = Animator.StringToHash("ItemType");
    //===============================================//
    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
    //
    private void OnEnable()
    {
        anim.SetFloat(hashItemType, (float)type);
    }
    //===============================================//
    public void Activate()
    {
        switch (type)
        {
            case ItemType.RESOURCE:
                UIManager.instance.AddRSC(resourceItemValue);
                break;
            case ItemType.AJPACK:
                UIManager.instance.AddAJPack();
                break;
            case ItemType.SHIELD:
                UIManager.instance.AddShield();
                break;
            case ItemType.SLOW:
                GameManager.instance.elecShooter.superViser.SpeedLVDown();
                break;
            default:
                break;
        }
    }
    //
    public void SetItem(ItemType value)
    { // ItemManager
        type = value;
        float f = (float)type;
        anim.SetFloat(hashItemType, f);
        SoundManager.instance.Play(SoundKey.ITEM_SHOW);
    }
    //
    //public void ShowItem()
    //{ // Call By Display
    //    anim.SetTrigger(hashShowItem);
    //    SoundManager.instance.Play(SoundKey.ITEM_SHOW);
    //}
    //
    public void ResetItem()
    { // Anim Frame Event : Item_Obtain : End frame
        SetDisplayNumber();
        // 초기화
        SetItem(ItemType.RESOURCE);
        //
        transform.position = Vector3.zero;
        //
        transform.SetParent(ItemManager.instance.itemHolder);
        //
        gameObject.SetActive(false);
    }
    //
    public void SetDisplayNumber()
    {
        transform.parent.GetComponent<Display>().SetDisplayNumber();
    }
    //===============================================//
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            anim.SetTrigger(hashObtain);
            SoundManager.instance.Play(SoundKey.ITEM_OBTAIN);
            Activate();
        }
    }
    //===============================================//
}