using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    //==========================================//
    public Transform target;
    private Transform tf;
    private Vector3 temp;
    //==========================================//
    private void Awake()
    {
        tf = GetComponent<Transform>();
    }
    //==========================================//
    void Update()
    {
        Move();
    }
    //==========================================//
    private void Move()
    {
        if (target)
        {
            temp = target.position;
            temp.z = tf.position.z;

            tf.position = temp;
        }
    }
}
