using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arm : MonoBehaviour
{
    //=======================================//
    // Parent Object
    public Dummy dummy;
    //=======================================//
    // Components
    private SpriteRenderer sr;
    //=======================================//
    // Child Object : JumpPack
    private AirJumpPack ajp;
    //=======================================//
    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }
    //=======================================//
    private void Update()
    {
        // Debug
        if (Input.GetKeyDown(KeyCode.J))
            UIManager.instance.AddAJPack();
        if (Input.GetKeyDown(KeyCode.K))
            UIManager.instance.UseAirJump();
    }
    //
    public void FlipX(bool value)
    {
        sr.flipX = value;
        if(ajp != null)
            ajp.Flip(value);
    }
    //
    public void UseJumpPack()
    {
        if(ajp != null)
            ajp.Use();
    }
    //
    public void EquipJumpPack(bool b, AirJumpPack value = null)
    {
        if (b)
            BeginJumpPack(value);
        else
            EndJumpPack();
    }
    //
    private void EndJumpPack()
    {
        dummy.SetJumpPackLayer(false);
        //
        ajp.RemoveAJP();
        //
        ajp = null;
    }
    //
    private void BeginJumpPack(AirJumpPack value)
    {
        // Set Dummy Anim Layer
        dummy.SetJumpPackLayer(true);
        // Set AirJumpPack Cach
        ajp = value;
        // Set To Child, Position, Activate, Shine Effect
        ajp.Set(transform, sr.flipX);
    }
    //
}
