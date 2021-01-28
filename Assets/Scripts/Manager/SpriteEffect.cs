using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteEffect : MonoBehaviour
{
    private Animator anim;
    //
    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
    private void Update()
    {
        isPlaying("ScanAnim");
    }
    //
    private bool isPlaying(string stateName)
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName(stateName)
            && anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
            return true;
        else
        {
            transform.SetParent(EffectManager.instance.scanHolder);
            gameObject.SetActive(false);
            transform.position = Vector3.zero;
            //
            return false;
        }
    }
}
