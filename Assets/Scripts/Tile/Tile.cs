using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    //=============================================//
    public Closet closet;
    public Inner inner;
    //=============================================//
    //private void Start()
    private void Awake()
    {
        closet = transform.GetChild(0).GetComponent<Closet>();
        inner = transform.GetChild(1).GetComponent<Inner>();
        //
        closet.SetTile(this);
    } // End Start
    //=============================================//
}
