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
    private Material mat;
    //=======================================//
    // Child Object : JumpPack
    private AirJumpPack ajp;
    //=======================================//
    private WaitForSeconds ws;
    //=======================================//
    // Dissolve
    private readonly int hashDissolveAmount = Shader.PropertyToID("_DissolveAmount");
    public float dissolveSpeed = 1.0f;
    private float dissolveValue = 0;
    // Dissolve
    //=======================================//
    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        mat = GetComponent<Material>();
        //
        ws = new WaitForSeconds(0.1f);
    }
    //=======================================//
    private void Update()
    {
        // Debug
        if (Input.GetKeyDown(KeyCode.F2))
            EquipJumpPack(true);
        if (Input.GetKeyDown(KeyCode.F3))
            EquipJumpPack(false);
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
        ajp.Use();
    }
    //
    public void EquipJumpPack(bool b, AirJumpPack value = null)
    {
        if (b)
            StartCoroutine(BeginJumpPack(value));
        else
            StartCoroutine(EndJumpPack());
    }
    //
    private IEnumerator EndJumpPack()
    {
        yield return ws;
        // Inactivate After
        dummy.SetJumpPackLayer(false);
        //
        ajp.ResetAJP();
        ajp = null;
    }
    private IEnumerator BeginJumpPack(AirJumpPack value)
    {
        // Activate first
        dummy.SetJumpPackLayer(true);
        //
        ajp = value;
        //
        ajp.Set(transform);
        //
        yield return null;
    }
    private IEnumerator DissolveEffect()
    {
        while (true)
        {
            mat.SetFloat(hashDissolveAmount, dissolveValue);
            //
            if (dissolveValue < 1.0f)
            {
                dissolveValue += Time.deltaTime * dissolveSpeed;
                yield return null;
            }
            else
            {
                dissolveValue = 0.0f;
                yield break;
            }
        }
    }
    //=======================================//
}
