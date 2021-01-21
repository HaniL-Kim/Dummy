using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eye : MonoBehaviour
{
    public Dummy dummy;
    // Flags
    public bool isScan;
    // Component
    Transform tf;
    SpriteRenderer rend;
    Animator anim;
    // Transform
    public enum EyeState { NORMAL, UP, DOWN, CROUCH };
    //
    Vector3[] eyePos = new Vector3[4]; // Local positions by EyeState

    void Start()
    {
        tf = GetComponent<Transform>();
        rend = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        //
        eyePos[0] = new Vector3(0, 0, -1.0f);
        eyePos[1] = new Vector3(0, 2.0f, -1.0f);
        eyePos[2] = new Vector3(0, -2.0f, -1.0f);
        eyePos[3] = new Vector3(0, -3.0f, -1.0f);
    } // End Start

    public void Scan()
    {
        anim.SetTrigger("isScan");
    } // End Scan

    public void SetIsScan(int value)
    { // Animation Trigger Event
        dummy.isScan = System.Convert.ToBoolean(value);
    } // End SetIsScan

    public void FlipX(bool value)
    {
        rend.flipX = value;
    } // End FlipX

    public void SetPos(EyeState key)
    {
        Vector3 temp = eyePos[(int)key];
        if (key == EyeState.UP || key == EyeState.DOWN)
        {
            if (dummy.isScan == true)
                temp.y *= 0.5f;
        }
        tf.localPosition = temp;
    } // End SetPos

    public void SetIdle(bool value)
    {
        anim.SetBool("isIdle", value);
    } // End SetIdle

} // End of Script Eye