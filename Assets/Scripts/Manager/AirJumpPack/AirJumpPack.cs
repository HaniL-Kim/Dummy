using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirJumpPack : MonoBehaviour
{
    //====================================//
    private SpriteRenderer sr;
    private Animator anim;
    //====================================//
    private readonly int hashFireJumpPack = Animator.StringToHash("FireJumpPack");
    //====================================//
    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }
    //====================================//
    public void Set(Transform value)
    {
        transform.SetParent(value);
        gameObject.SetActive(true);
        //
        transform.localPosition = new Vector3(0, 0, 1);
    }
    //
    public void ResetAJP()
    {
        transform.SetParent(ItemManager.instance.jumpPackHolder);
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
    }
    //====================================//
}
