using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirJumpPack : MonoBehaviour
{
    //====================================//
    // Components
    private SpriteRenderer sr;
    private Animator anim;
    private Material mat;
    //====================================//
    // Shine
    [ColorUsageAttribute(true, true)]
    public Color shineColor;
    public float shineSpeed = 7.0f;
    //
    private readonly int hashOutlineColorInner = Shader.PropertyToID("_OutlineColor_Inner");
    private readonly int hashOutlineTK = Shader.PropertyToID("_OutlineThickness");
    //
    public Transform shineEffect;
    public Transform shine; // 17 ~ -11 (28)
    public SpriteRenderer sr_shine;
    //
    private Vector3 shineNormalScale = new Vector3(+1, 1, 1);
    private Vector3 shineFlipScale = new Vector3(-1, 1, 1);
    private Vector3 shineStartPos = new Vector3(17, 17, 0);
    //====================================//
    // Dissolve
    private readonly int hashDissolveAmount = Shader.PropertyToID("_DissolveAmount");
    public float dissolveSpeed = 1.0f;
    private float dissolveValue = 0;
    //====================================//
    // Anim
    private readonly int hashFireJumpPack = Animator.StringToHash("FireJumpPack");
    //=======================================//
    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        mat = GetComponent<Renderer>().material;
        //
        SetOutlineColor();
    }
    //====================================//
    private void SetOutlineColor()
    {
        sr_shine.material.SetColor(hashOutlineColorInner, shineColor);
        sr_shine.material.SetFloat(hashOutlineTK, 2.0f);
    }
    //
    private IEnumerator ShineEffect()
    {
        Vector3 dir = new Vector3(-1, -1, 0);
        //
        float speed = shineSpeed;
        float halfDist = Vector3.Distance(shineStartPos, Vector3.zero);
        while (true)
        {
            float d = Vector3.Distance(shineStartPos, shine.localPosition);
            if (d < halfDist)
                speed *= 1.10f;
            else
                speed *= 0.90f;

            if(speed <= 1.0f)
            {
                shine.localPosition = shineStartPos; 
                yield break;
            }
            else
                shine.localPosition += (dir * Time.deltaTime * speed);
            //
            yield return null;
        }
    }
    //====================================//
    public void Set(Transform value, bool flip)
    {
        transform.SetParent(value);
        //
        transform.localPosition = new Vector3(0, 0, 1);
        //
        Flip(flip);
        //
        gameObject.SetActive(true);
        //
        StartCoroutine(ShineEffect());
    }
    //
    public void ResetAJP()
    {
        transform.SetParent(ItemManager.instance.jumpPackHolder);
        GetComponentInChildren<Outline>().outline.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }
    //
    public void Use()
    {
        anim.SetTrigger(hashFireJumpPack);
    }
    //
    public void Flip(bool value)
    {
        sr.flipX = value;
        //
        if (value)
            shineEffect.localScale = shineFlipScale;
        else
            shineEffect.localScale = shineNormalScale;
    }
    //
    public void RemoveAJP()
    {
        StartCoroutine(DissolveEffect());
    }
    //=======================================//
    private IEnumerator DissolveEffect()
    {
        GetComponentInChildren<Outline>().outline.gameObject.SetActive(false);
        //
        while (true)
        {
            if (dissolveValue < 1.0f)
            {
                dissolveValue += Time.deltaTime * dissolveSpeed;
                mat.SetFloat(hashDissolveAmount, dissolveValue);
                yield return null;
            }
            else
            {
                ResetAJP();
                //
                dissolveValue = 0.0f;
                mat.SetFloat(hashDissolveAmount, dissolveValue);
                //
                yield break;
            }
        }
    }
    //====================================//
}
