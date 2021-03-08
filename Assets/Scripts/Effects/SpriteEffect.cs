using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EffectType
{
    NONE, SCAN, EXPLOSION, FLAG,
    PULL, PUSH, THUNDER, END
}
//
public class SpriteEffect : MonoBehaviour
{
    //=================================================//
    public EffectType type;
    //
    protected Animator anim;
    protected Collider2D col;
    //=================================================//
    protected void Awake()
    {
        anim = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
    }
    //
    protected void Update()
    {
        ControlAnim();
    }
    //=================================================//
    protected void ControlAnim()
    {
        switch (type)
        {
            case EffectType.SCAN:
                {
                    if (isPlaying("ScanAnim") == false)
                        Reset();
                }
                break;
            case EffectType.EXPLOSION:
                {
                    if (isPlaying("ExplosionAnim") == false)
                        Reset();
                }
                break;
            default:
                break;
        }
    }
    //
    protected bool isPlaying(string stateName)
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName(stateName)
            && anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
            return true;
        else
            return false;
    }
    //
    public void InActivate()
    {
        transform.gameObject.SetActive(false);
    }
    //
    protected void InactiveCollider()
    {
        col.enabled = false;
    }
    //
    public void Reset()
    {
        if (col)
            col.enabled = true;
        //
        transform.position = Vector3.zero;
        transform.SetParent(EffectManager.instance.effHolders[(int)type]);
        gameObject.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (type == EffectType.EXPLOSION)
        {
            if (collision.CompareTag("FootBoard"))
            {
                collision.transform.GetComponent<FootBoard>().DestroyFootBoard();
                return;
            }
        }
    }
    //=================================================//
}
