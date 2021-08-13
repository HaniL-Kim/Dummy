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
    private readonly int hashFlip = Animator.StringToHash("Flip");
    //=============================================//
    // Shake
    public Shaker shaker;
    //
    public bool isFliped = false;
    //=============================================//
    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
    //=============================================//
    public void ResetCloset()
    {
        isFliped = false;
        //
        shaker.ResetShake();
        //
        anim.SetBool(hashFlip, false);
        //
        //inner.ResetInner();
        //
        GetComponent<SpriteRenderer>().color = Color.white;
        transform.gameObject.SetActive(true);
    }
    //=============================================//
    public void SetTile(Tile value)
    {
        tile = value;
        inner = tile.inner;
        inner.closet = this;
    }
    //=============================================//
    public void HitFlag()
    {
        shaker.StartShake();
        //
        inner.display.flagHit = true;
        //
        Invoke("Flip", shaker.time);
    }
    //
    public void Flip()
    { // Trigger Enter Event
        isFliped = true;
        anim.SetBool(hashFlip, true);
        inner.PlayFlipSound();
        // SoundManager.instance.Play(SoundKey.FLIP);
    }
    //
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
        {
            if(collision.GetComponent<Dummy>().isInteractable == true)
                Flip();
        }
    } // End OnTriggerEnter2D
    //
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isFliped == true)
            return;
        //
        if (collision.CompareTag("Player") == true)
        {
            if(collision.GetComponent<Dummy>().isInteractable == true)
                Flip();
        }
    } // End OnTriggerStay2D
}
