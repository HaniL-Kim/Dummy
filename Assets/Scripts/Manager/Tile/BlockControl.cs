using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class BlockControl : MonoBehaviour
{
    public Block block;
    //=========================================//
    public Transform tf_bottom, tf_top;
    //=========================================//
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(tf_bottom.position, tf_bottom.right * 500);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(tf_top.position, tf_top.right * 500);
    }
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
    public void ResetBlock()
    {
        if(Map.instance.isInfinite == false)
        {
            gameObject.SetActive(false);
            return;
        }
        //
        // Set Pos //
        // Move to over Top Block
        Vector3 temp = transform.position;
        temp.y += Map.BlockHeight * 3.0f;
        transform.position = temp;
        //
        // a. Reset Tiles //
        Tile[] tiles = GetComponentsInChildren<Tile>();
        foreach (var t in tiles)
        {
            t.ResetTile();
        }
        // b. Reset Concrete Tiles //
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
        // c. Reset Mine // 
        Map.instance.SetMine(block);
        // d. Reset ArroundTileInfo //
        Invoke("ReSetMineInfo", 0.1f);
    }
    //
    private void ReSetMineInfo()
    {
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
    }
    //=========================================//
}
