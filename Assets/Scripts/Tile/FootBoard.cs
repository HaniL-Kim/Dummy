using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootBoard : MonoBehaviour
{
    //=============================================//
    public bool canDistroy = false;
    //=============================================//
    private SpriteRenderer sr;
    //=============================================//
    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }
    //=============================================//
    public void Set()
    {
        sr.enabled = true;
        canDistroy = true;
    }
    public void Destroy()
    {
        // TODO
        // Anim
    }
    //=============================================//
}
