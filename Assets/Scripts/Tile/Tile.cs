using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    //=============================================//
    public Closet closet;
    public Inner inner;
    public Resource resource;
    //=============================================//
    private void Awake()
    {
        closet = transform.GetChild(0).GetComponent<Closet>();
        inner = transform.GetChild(1).GetComponent<Inner>();
        //
        closet.SetTile(this);
    }
    //=============================================//
    public void ResetTile()
    {
        closet.ResetCloset();
        resource.ResetResource();
    }
    //=============================================//
}
