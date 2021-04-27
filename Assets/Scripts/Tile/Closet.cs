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
    public bool isShake = false;
    private Vector3 originPosition;
    private Quaternion originRotation;
    //
    private float shake_decay, shake_intensity, shake_time;
    private const float cDecay = 0.002f, cIntensity = 0.3f, cTime = 0.33f;
    //
    public bool isFliped = false;
    //=============================================//
    private void Awake()
    {
        anim = GetComponent<Animator>();
    } // End Start
    //=============================================//
    private void Update()
    {
        Shake();
    }
    //=============================================//
    public void ResetCloset()
    {
        isFliped = false;
        //
        isShake = false;
        shake_decay = cDecay;
        shake_intensity = cIntensity;
        shake_time = cTime;
        //
        anim.SetBool(hashFlip, false);
        //
        inner.ResetInner();
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
    } // End SetTile
    //=============================================//
    public void HitFlag()
    {
        StartShake();
        inner.display.flagHit = true;
    }
    //
    public void Flip()
    { // Trigger Enter Event
        isFliped = true;
        anim.SetBool(hashFlip, true);
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
    private void Shake()
    {
        if (isShake == false)
            return;
        //
        if (shake_time > 0)
        {
            transform.position = originPosition + Random.insideUnitSphere * shake_intensity;
            transform.rotation = new Quaternion(
            originRotation.x + Random.Range(-shake_intensity, shake_intensity) * .2f,
            originRotation.y + Random.Range(-shake_intensity, shake_intensity) * .2f,
            originRotation.z + Random.Range(-shake_intensity, shake_intensity) * .2f,
            originRotation.w + Random.Range(-shake_intensity, shake_intensity) * .2f);
            shake_intensity -= shake_decay;
            shake_time -= Time.deltaTime;
        }
        else
        {
            isShake = false;
            Flip();
        }
    }
    //
    public void StartShake()
    {
        isShake = true;
        originPosition = transform.position;
        originRotation = transform.rotation;
    } // End StartShake()
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
