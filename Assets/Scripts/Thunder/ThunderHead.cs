using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ThunderHead : MonoBehaviour
{
    //====================================//
    public Transform tf;
    public SpriteRenderer sr;
    public Animator anim;
    //====================================//
    private readonly int hashHeadReady =
        Animator.StringToHash("ThunderReady");
    //====================================//
    private Vector3 temp;
    //====================================//
    public bool ready;
    //====================================//
    private void Awake()
    {
        ready = false;
    }
    //====================================//
    public void ColorTween(int id_color, Color c, float t)
    {
        sr.material.DOColor(c, id_color, t);
            //.SetId(transform.name + "_TweenMatColor");
    }
    //
    public float GetLocalPosY()
    {
        return tf.localPosition.y;
    }
    //
    public float GetPosY()
    {
        return tf.position.y;
    }
    //
    public void SetPosY(float posY)
    {
        temp = tf.position;
        temp.y = posY;
        tf.position = temp;
    }
    //
    public void SetMat(int id_color, Color c, int id_thickness, float tk)
    {
        sr.material.SetColor(id_color, c);
        sr.material.SetFloat(id_thickness, tk);
    }
    //
    public void ResetThunderHead(int id_color, Color c)
    {
        ready = false;
        //
        tf.localPosition = new Vector3(0, 0, -1);
        //
        sr.DOKill();
        sr.color = Color.white;
        sr.material.SetColor(id_color, c);
        //
        anim.enabled = true;
    }
    //
    public void Explode(Color c, float t)
    {
        anim.enabled = false;
        sr.DOColor(c, t);
    }
    //
    public void FollowHead(string op, float elecPos, Vector3 dir, float speed)
    {
        if (ready == false)
        {
            bool b = false;
            if(op == "<=")
                b = tf.position.y <= elecPos;
            else
                b = tf.position.y >= elecPos;
            //
            if (b)
                tf.Translate(dir * speed * Time.deltaTime);
            else
            {
                ready = true;
                anim.SetTrigger(hashHeadReady);
            }
        }
    }
}