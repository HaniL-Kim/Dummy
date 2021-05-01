using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;


public class BlockControl : MonoBehaviour
{
    public Block block;
    //=========================================//
    public Transform tf_bottom, tf_top;
    //=========================================//
    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.blue;
    //    Gizmos.DrawRay(tf_bottom.position, tf_bottom.right * 500);
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawRay(tf_top.position, tf_top.right * 500);
    //}
    //=========================================//
    public void SetBorder(Transform b, Transform t)
    {
        tf_bottom = b;
        tf_top = t;
    }
    //
    public void ActivateBlock()
    {
        gameObject.SetActive(true);
    }
    //
    private void SetBlockPos()
    {
        Vector3 temp = transform.position;
        temp.y += Map.BlockHeight * 3.0f;
        transform.position = temp;
    }
    //
    private void ResetTiles()
    {
        int floorCount = block.floors.Count, tileCount = block.GetFloor(0).tiles.Count;
        //
        for (int i = 0; i < floorCount; ++i)
        {
            for (int j = 0; j < tileCount; ++j)
            {
                Tile t = block.GetFloor(i).tiles[j].GetComponent<Tile>();
                t.ResetTile();
            }
        }
        /*
            foreach (var t in GetComponentsInChildren<Tile>())
            t.ResetTile();
        */
    }
    private void ResetConcretes()
    {
        foreach (Floor floor in block.floors)
        {
            // Reset Concretes
            foreach (GameObject concObj in floor.concreteTiles)
                concObj.GetComponent<Concrete>().SetToOrigin();
            // Swap CrashConcrete And Random Tile
            Transform crash = floor.concreteTiles[2].transform;
            Transform tile = floor.tiles[Random.Range(0, 6)].transform;
            //
            MyUtility.SwapPos(crash, tile);
            MyUtility.SwapNameAt(crash, tile, 0);
        }
    }
    //
    public void ResetBlock()
    {
        if(Map.instance.isInfinite == false)
        { // 1. Normal Mode
            gameObject.SetActive(false);
            return;
        }
        // 2. Infinty Mode
        // a. Set Block Pos : Move to over Top Block //
        SetBlockPos();
        // b. Reset Tiles //
        ResetTiles();
        // c. Reset Concrete Tiles //
        ResetConcretes();
        // d. Reset Mine // 
        Sequence sq = DOTween.Sequence()
            .AppendCallback(() => { Map.instance.SetMine(block); })
            .AppendCallback(() => { ReSetMineInfo(); })
            ;
        //Map.instance.SetMine(block);
        // e. Reset ArroundTileInfo //
        //ReSetMineInfo();
        //Invoke("ReSetMineInfo", 0.1f);
    }
    //
    private void ReSetMineInfo()
    {
        Debug.Log("ReSetMineInfo : begin");
        //
        int blockNum = (transform.name[0]) - '1';
        int belowBlockNum = (blockNum + 2) % (Map.instance.blocks.Count - 1); // 3
        //
        Block belowBlock = Map.instance.blocks[belowBlockNum + 1];
        Floor belowHighestFloor = belowBlock.floors.Last();
        //
        // Debug.Break();
        Map.instance.SetArroundMineInfo(block);
        Map.instance.SetArroundMineInfo(belowHighestFloor);
        // InActivate(Ready) //
        gameObject.SetActive(false);
        //
        Debug.Log("ReSetMineInfo : End");
    }
    //=========================================//
}
