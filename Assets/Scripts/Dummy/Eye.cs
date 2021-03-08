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
    SpriteRenderer sr;
    Animator anim;
    //
    private readonly int noRSCHash = Animator.StringToHash("NoResource");
    private readonly int usingSkillHash = Animator.StringToHash("UsingSkill");
    // Transform
    public enum EyeState { NORMAL, UP, DOWN, CROUCH };
    public EyeState state = EyeState.NORMAL;
    //
    Vector3[] eyePos = new Vector3[4]; // Local positions by EyeState

    void Start()
    {
        tf = GetComponent<Transform>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        // Normal, Up, Down, Crouch
        eyePos[0] = new Vector3(0, 0, -1.0f);
        eyePos[1] = new Vector3(0, 1.0f, -1.0f);
        eyePos[2] = new Vector3(0, -1.0f, -1.0f);
        eyePos[3] = new Vector3(0, -3.0f, -1.0f);
    }
    //
    public void PlayEyeNoRSCAnim()
    {
        anim.SetTrigger(noRSCHash);
    }
    //
    public void PlayEyeSkillAnim()
    {
        anim.SetTrigger(usingSkillHash);
    }
    //
    public void EndSkill()
    {
        dummy.usingSkill = false;
    }
    //
    public void FlipX(bool value)
    {
        sr.flipX = value;
    } // End FlipX
    //
    public void SetState(EyeState key)
    {
        state = key;
        SetPos();
    }

    public void SetPos()
    {
        Vector3 temp = eyePos[(int)state];
        if (state == EyeState.UP || state == EyeState.DOWN)
        {
            if(anim.GetCurrentAnimatorStateInfo(0).IsName("Normal"))
                temp.y *= 2.0f;
        }
        //
        tf.localPosition = temp;
    } // End SetPos

    public void SetIdle(bool value)
    {
        anim.SetBool("isIdle", value);
    } // End SetIdle

} // End of Script Eye