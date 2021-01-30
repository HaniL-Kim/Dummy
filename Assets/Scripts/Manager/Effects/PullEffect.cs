using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullEffect : SpriteEffect
{
    //===============================================//
    public int repeatCount = 5;
    public int playCount = 0;
    public float pullSpeed = 50.0f;
    Vector3 dirToEffect;
    //===============================================//
    new private void Update()
    {
        CheckPlayCount();
    }
    //===============================================//
    public void AddPlayCount()
    { // Anim Frame Event
        ++playCount;
    }
    //===============================================//
    private void CheckPlayCount()
    {
        if (playCount == repeatCount)
        {
            EffectManager.instance.Play("Explosion", transform);
            base.Reset();
            playCount = 0;
        }
    }
    //
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            print("Pull Player");
            dirToEffect = (transform.position - collision.transform.position).normalized;
            collision.transform.Translate(dirToEffect * pullSpeed * Time.deltaTime, Space.World);
        }
    }
    //===============================================//
}
