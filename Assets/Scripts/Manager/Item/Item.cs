﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    //===============================================//
    public enum ItemType { RESOURCE, AJPACK, SHIELD, SLOW }
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
                GameManager.instance.elecShooter.SlowSpeed();
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
    }
    //
    public void ShowItem()
    { // Call By Display
        anim.SetTrigger(hashShowItem);
    }
    //
    public void ResetItem()
    {
        SetItem(ItemType.RESOURCE);
        gameObject.SetActive(false);
    }
    //===============================================//
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            anim.SetTrigger(hashObtain);
            Activate();
        }
    }
    //===============================================//
}