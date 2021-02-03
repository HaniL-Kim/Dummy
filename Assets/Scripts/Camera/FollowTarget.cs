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
    //==========================================//
    private void Awake()
    {
        tf = GetComponent<Transform>();
        float halfCamHeight = Camera.main.orthographicSize;
        float halfCamWidth = halfCamHeight * Camera.main.aspect;
        halfSize = new Vector2(halfCamWidth, halfCamHeight);
    }
    //==========================================//
    void Update()
    {
        SetBorder();
        Move();
        LimitToBorder();
    }
    //==========================================//
    private void OnDrawGizmos()
    {
        // Draw Border
        {
            Gizmos.color = Color.red;
            if (camBorder_Up)
            {
                Vector3 start = camBorder_Up.position;
                start.x = -272.0f;
                Vector3 dir = camBorder_Up.position;
                dir.x = 304.0f;
                Gizmos.DrawLine(start, dir);
            }
            if (camBorder_Down)
            {
                Vector3 start = camBorder_Down.position;
                start.x = -272.0f;
                Vector3 dir = camBorder_Down.position;
                dir.x = 304.0f;
                Gizmos.DrawLine(start, dir);
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
}
