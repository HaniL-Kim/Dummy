using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagEffect : SpriteEffect
{
    //===================================//
    private Transform tfStart; // for Debug
    //
    private Transform tf;
    private Transform targetTile;
    //===================================//
    private readonly int hashExplode = Animator.StringToHash("isExplode");
    //===================================//
    private Vector3 dir = Vector2.zero;
    private float speed = 0;
    private float lifeTime = 0;
    //===================================//
    new private void Awake()
    {
        base.Awake();
        //
        tf = GetComponent<Transform>();
        anim = GetComponent<Animator>();
    }
    //===================================//
    new private void Update()
    {
        base.Update();
        //
        Move();
        Explode();
    }
    //===================================//
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(tfStart.position, tf.position);
    }
    //===================================//
    private void Move()
    {
        if (lifeTime <= 0)
            return;
        //
        Vector3 pos = dir * speed * Time.deltaTime;
        tf.position += pos;
        //tf.Translate(dir * speed * Time.deltaTime, Space.Self);
    } // Move()
    private void Explode()
    {
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0)
            anim.SetTrigger(hashExplode);

    } // Explode()
    public void Set(Transform start, Transform end, float arriveTime)
    {
        tfStart = start;
        lifeTime = arriveTime;
        //
        Vector3 startPos = start.position;
        startPos.z = -5.0f;
        //
        targetTile = end;
        Vector3 endPos = end.position;
        endPos.z = -5.0f;
        //
        tf.position = startPos;
        //
        dir = endPos - startPos;
        float dist = dir.magnitude;
        //
        speed = dist / arriveTime;
        dir.Normalize();
        //
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        tf.rotation = Quaternion.Euler(0.0f, 0.0f, angle);
    } // Set()
    public void Reset()
    { // Anim Trigger Event
        tf.gameObject.SetActive(false);
        targetTile.GetComponent<Closet>().HitFlag();
    }
}
