using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Closet : MonoBehaviour
{
    //=============================================//
    private Animator anim;
    //=============================================//
    private Tile tile;
    private Inner inner;
    //=============================================//
    void Start()
    {
        anim = GetComponent<Animator>();
    } // End Start
    //=============================================//
    public void SetTile(Tile value)
    {
        tile = value;
        inner = tile.inner;
    } // End SetTile
    //=============================================//
    public void Flip()
    { // Trigger Enter Event
        anim.SetBool("Flip", true);
    } // End Flip
    public void FlipInner()
    { // Flip Anim Keyframe Event
        if (inner == null)
        { // exception_1
            Debug.LogError("Inner is null");
            return;
        }
        //
        transform.gameObject.SetActive(false);
        //
        tile.inner.Flip();
    } // End FlipInner
    //=============================================//
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") == true)
            Flip();
    } // End OnTriggerEnter2D
}
