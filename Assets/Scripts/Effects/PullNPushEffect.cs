using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullNPushEffect : SpriteEffect
{
    //===============================================//
    public int alertCount = 5;
    [SerializeField]
    private int curPlayCount;
    public float pullSpeed = 50.0f;
    public float pushSpeed = 50.0f;
    //
    Vector3 dirToEffect = Vector3.zero;
    //
    public List<GameObject> effects = new List<GameObject>();
    //===============================================//
    private new void Awake()
    {
        base.Awake();
        curPlayCount = alertCount;
    }
    //===============================================//
    private void CheckPlayCount()
    {
        --curPlayCount;
        if (curPlayCount == 0)
        {
            curPlayCount = alertCount;
            //
            foreach (GameObject effect in effects)
                effect.SetActive(false);
            //
            base.Reset();
        }
    }
    //
    private void ActivateEffect()
    {
        foreach (GameObject effect in effects)
        {
            if (effect.activeSelf == false)
            {
                effect.SetActive(true);
                return;
            }
        }
    }
    //
    private void PullPlayer(Transform player)
    {
        dirToEffect = Vector3.zero;
        dirToEffect.x = (transform.position.x - player.position.x);
        //if (Mathf.Abs(dirToEffect.x) <= 1.0f)
        float playerHalfWidth = player.GetComponent<BoxCollider2D>().bounds.size.x / 2.0f;
        if (Mathf.Abs(dirToEffect.x) <= playerHalfWidth)
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
        dirToEffect.z = player.position.z;
        //
        Dummy dm = player.GetComponent<Dummy>();
        if(dm.isJump)
            dirToEffect.y = (player.position.y - transform.position.y);
        else
            dirToEffect.y = 0;
        //
        dirToEffect.Normalize();
        //
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
