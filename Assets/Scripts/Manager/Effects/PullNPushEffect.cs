using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullNPushEffect : SpriteEffect
{
    //===============================================//
    public int repeatCount = 5;
    public int playCount = 0;
    public float pullSpeed = 50.0f;
    public float pushSpeed = 50.0f;
    //
    Vector3 dirToEffect = Vector3.zero;
    //===============================================//
    new private void Update()
    {
        CheckPlayCount();
        Debug.DrawRay(transform.position, dirToEffect * 20.0f, Color.red);
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
            //SetExplosion();
            playCount = 0;
            base.Reset();
        }
    }
    //
    private void PullPlayer(Transform player)
    {
        dirToEffect = Vector3.zero;
        dirToEffect.x = (transform.position.x - player.position.x);
        if (Mathf.Abs(dirToEffect.x) <= 1.0f)
            return;
        //
        dirToEffect.Normalize();
        dirToEffect.z = player.position.z;
        player.Translate(dirToEffect * pullSpeed * Time.deltaTime, Space.World);
    }
    //
    private void PushPlayer(Transform player)
    {
        dirToEffect = Vector3.zero;
        dirToEffect.x = (player.position.x - transform.position.x);
        dirToEffect.y = (player.position.y - transform.position.y);
        dirToEffect.Normalize();
        dirToEffect.z = player.position.z;
        player.Translate(dirToEffect * pushSpeed * Time.deltaTime, Space.World);
    }
    //===============================================//
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            switch (type)
            {
                case EffectType.PULL:
                    PullPlayer(collision.transform);
                    break;
                case EffectType.PUSH:
                    PushPlayer(collision.transform);
                    break;
                default:
                    break;
            }
        }
    }
    //===============================================//
}
