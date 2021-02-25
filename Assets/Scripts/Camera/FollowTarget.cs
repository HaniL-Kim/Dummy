using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    //==========================================//
    public Transform target; // Player
    //
    public Transform camBorder_Up;
    public Transform camBorder_Down;
    public Vector2 minPos = Vector2.zero; // Border_Down
    public Vector2 maxPos = Vector2.zero; // Border_Up
    //
    private Vector2 halfSize = Vector2.zero; // CamSize
    //
    private Transform tf;
    private Vector3 targetPos = Vector2.zero;
    //
    public bool blockCulling = false;
    public float blockCullDist;
    //==========================================//
    private void Awake()
    {
        tf = GetComponent<Transform>();
        //
        float halfCamHeight = Camera.main.orthographicSize;
        float halfCamWidth = halfCamHeight * Camera.main.aspect;
        halfSize = new Vector2(halfCamWidth, halfCamHeight);
        //
        blockCullDist = 3 * Map.FloorHeight;
    }
    //==========================================//
    void Update()
    {
        SetBorder();
        //
        Move();
        //
        LimitToBorder();
        //
        BlockCulling();
    }
    //==========================================//
    private void OnDrawGizmos()
    {
        // Draw Border
        {
            Gizmos.color = Color.yellow;
            if (camBorder_Up)
            {
                Gizmos.DrawRay(camBorder_Up.position, Vector3.left * 348.0f * 2.0f);
            }
            if (camBorder_Down)
            {
                Gizmos.DrawRay(camBorder_Down.position, Vector3.left * 348.0f * 2.0f);
            }
        }
    }
    //==========================================//
    private void Move()
    {
        if (target)
        {
            targetPos = target.position;
            targetPos.z = tf.position.z;
            //
            tf.position = targetPos;
        }
    }
    //
    private void SetBorder()
    {
        //if (camBorder_Left && camBorder_Right)
        if (camBorder_Up && camBorder_Down)
        {
            //minPos.x = camBorder_Left.position.x - halfSize.x;
            minPos.y = camBorder_Down.position.y + halfSize.y;
            //maxPos.x = camBorder_Right.position.x + halfSize.x;
            maxPos.y = camBorder_Up.position.y - halfSize.y;
        }
    }
    //
    private void LimitToBorder()
    {
        if (targetPos.x <= minPos.x)
            targetPos.x = minPos.x;
        if (targetPos.x >= maxPos.x)
            targetPos.x = maxPos.x;
        //
        if (targetPos.y <= minPos.y)
            targetPos.y = minPos.y;
        if (targetPos.y >= maxPos.y)
            targetPos.y = maxPos.y;
        //
        tf.position = targetPos;
    }
    //============================================//
    private void BlockCulling()
    {
        if (blockCulling == false)
            return;
        //
        float ct = camBorder_Up.position.y;
        float cb = camBorder_Down.position.y;
        //
        Map m = Map.instance;
        for (int i = 1; i < m.blocks.Count; ++i)
        { // i : 0 -> ReadyBlock
            BlockControl b = m.blocks[i].block.GetComponent<BlockControl>();
            //
            float bt = b.tf_top.position.y;
            float bb = b.tf_bottom.position.y;
            //
            float dist_BB_CT = bb - ct;
            float dist_CB_BT = cb - bt;
            // Block is Above Camera
            if (dist_BB_CT > 0 && dist_BB_CT < blockCullDist)
            {
                if (b.gameObject.activeSelf == false)
                    b.ActivateBlock();
            }
            // Block is Below Camera
            else if (dist_CB_BT > 0 && dist_CB_BT > blockCullDist)
            {
                if (b.gameObject.activeSelf == true)
                    b.ResetBlock();
            }
        }
    }
}
