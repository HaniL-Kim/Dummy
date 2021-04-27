﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RotaryHeart;
using MyUtilityNS;

//////////////////////////////////////////
public class ItemManager : MonoBehaviour
{
    //=====================================//
    public static ItemManager instance;
    //=====================================//
    public const int cItemOrderCount = 100;
    public const int cItemCount = 100;
    //=====================================//
    public StringFloat itemRates = new StringFloat();
    //private List<int> listItemOrder = new List<int>();
    //[SerializeField]
    //private int order = 0;
    //=====================================//
    public Transform itemHolder;
    public GameObject itemPrefab;
    private List<GameObject> listItems = new List<GameObject>();
    //
    [HideInInspector]
    public Transform jumpPackHolder;
    public GameObject jumpPackPrefab;
    private List<GameObject> listJumpPacks = new List<GameObject>();
    //=====================================//
    private void Awake()
    {
        instance = this;
    }
    //
    void Start()
    {
        CreateItem();
        //
        // CreateItemOrder();
        //
        CreateJumpPack();
    }
    //=====================================//
    private void CreateJumpPack()
    {
        jumpPackHolder = new GameObject("JumpPackHolder").transform;
        jumpPackHolder.SetParent(transform);
        for(int i = 0; i < 5; ++i)
        {
            GameObject item = Instantiate(jumpPackPrefab, jumpPackHolder);
            item.SetActive(false);
            //
            listJumpPacks.Add(item);
        }
    }
    //
    private void CreateItem()
    {
        itemHolder = new GameObject("ItemHolder").transform;
        itemHolder.SetParent(transform);
        for(int i = 0; i < cItemCount; ++i)
        {
            GameObject item = Instantiate(itemPrefab, itemHolder);
            item.SetActive(false);
            //
            listItems.Add(item);
        }
    }
    /*
    private void CreateItemOrder()
    {
        int itemCount = 0, itemNum = 0;
        foreach (var data in itemRates)
        {
            itemCount = (int)(data.Value * (float)cItemOrderCount);
            for(int i = 0; i < itemCount; ++i)
                listItemOrder.Add(itemNum);
            //
            ++itemNum;
        }
        //
        MyUtility.ShuffleList(listItemOrder);
    }
    */
    //=====================================//
    public AirJumpPack GetJumpPack()
    {
        for(int i = 0; i < listJumpPacks.Count; ++i)
        {
            if (listJumpPacks[i].activeSelf == false)
                return listJumpPacks[i].GetComponent<AirJumpPack>();
        }
        //
        return null;
    }
    //
    private GameObject GetItem()
    {
        for(int i = 0; i < listItems.Count; ++i)
        {
            if (listItems[i].activeSelf == false)
                return listItems[i];
        }
        //
        return null;
    }
    //
    public void SetItemRate(int speedLV)
    {
        // itemRates["Resource"] = 0.2f; // 20%
        float slowRate = (float)speedLV * 0.06f; // lv / 6%
        itemRates["Slow"] = slowRate;
        float otherRate = (1.0f - itemRates["Resource"] - slowRate) * 0.5f;
        itemRates["AJPack"] = otherRate;
        itemRates["Shield"] = otherRate;
        //
        Debug.LogFormat("Set ItemRate of LV{0}", speedLV);
        //string temp = "";
        //foreach (var data in itemRates)
        //    temp += data.Key + "[" + data.Value.ToString() + "]" + "\n";
        //Debug.Log(temp);
    }
    //
    public void ShowItem(Transform tf)
    {
        GameObject item = GetItem();
        if (MyUtility.IsNull(item, "Item") == true)
            return;
        //
        int itemKey = MyUtility.GetValueFromRates(itemRates);
        Item.ItemType type = (Item.ItemType)itemKey;
        // Item.ItemType type = (Item.ItemType)listItemOrder[order];
        //
        item.SetActive(true);
        item.GetComponent<Item>().SetItem(type);
        //
        item.transform.SetParent(tf);
        item.transform.localPosition = Vector3.zero;
        //
        //order++;
        //if (order == cItemOrderCount)
        //{
        //    order = 0;
        //    MyUtility.ShuffleList(listItemOrder);
        //}
    }
    //=====================================//
}
