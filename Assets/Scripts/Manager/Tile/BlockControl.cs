using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockControl : MonoBehaviour
{
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
    //=========================================//
}
