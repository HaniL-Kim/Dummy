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
    private readonly int hashHitFlag = Animator.StringToHash("hitFlag");
    private readonly int hashFlip = Animator.StringToHash("Flip");
    //=============================================//
    // Shake
    public bool isShake = false;
    private Vector3 originPosition;
    private Quaternion originRotation;
    public float shake_decay;
    public float shake_intensity;
    public float shake_time;
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
        //anim.SetTrigger(hashHitFlag);
    }
    //
    public void Flip()
    { // Trigger Enter Event
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
        //if (shake_intensity > 0)
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
    } // End Shake()
    public void StartShake()
    {
        isShake = true;
        originPosition = transform.position;
        originRotation = transform.rotation;
        //shake_intensity = 0.3f;
        //shake_decay = 0.002f;
    } // End StartShake()
    //=============================================//
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") == true)
            Flip();
    } // End OnTriggerEnter2D
}
