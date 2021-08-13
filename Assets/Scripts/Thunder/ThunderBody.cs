using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ThunderBody : MonoBehaviour
{
    //=====================================//
    public Thunder thunder;
    //=====================================//
    // components
    public Transform tf;
    public SpriteRenderer sr;
    //public Animator anim;
    public BoxCollider2D col;
    //=====================================//
    public Sprite sprite_Normal;
    public Sprite sprite_Exp;
    //=====================================//
    private readonly int hashBodyExplode =
        Animator.StringToHash("ThunderExplode");
    //=====================================//
    Vector3 temp = Vector3.zero;
    //=====================================//
    public void ResetThunderBody()
    {
        Debug.Log("ResetThunderBody");
        //
        Vector2 temp = Vector2.zero;
        //
        sr.color = Color.white;
        //
        temp = sr.size;
        temp.y = 0;
        sr.size = temp;
        //
        temp = col.size;
        temp.y = 0;
        col.size = temp;
        //
        col.enabled = false;
        //
        thunder.ResetThunder();
    }
    //
    public void ResetThunderBody(int id_color, Color c)
    {
        transform.localPosition = Vector3.zero;
        sr.sprite = sprite_Normal;
        sr.color = Color.white;
        sr.material.SetColor(id_color, c);
    }
    //
    public void ColorTween(int id_color, Color c, float t)
    {
        sr.material.DOColor(c, id_color, t);
    }
    //
    public void SetMat(int id_color, Color c, int id_thickness, float tk)
    {
        sr.material.SetColor(id_color, c);
        sr.material.SetFloat(id_thickness, tk);
    }
    //
    public void Explode(int id_color, Color expColor, Color disColor, float t)
    {
        sr.sprite = sprite_Exp;
        sr.material.SetColor(id_color, expColor);
        col.enabled = true;
        //
        Tween tween_fade = sr.DOColor(disColor, t);
        Sequence exp = DOTween.Sequence()
        .Append(tween_fade)
        .AppendCallback(ResetThunderBody)
        .InsertCallback(0.3f, () => { col.enabled = false; })
        ;
    }
    //
    public void SetBody()
    {
        { // pos
            temp = Vector3.zero;
            temp.y =
                (thunder.th_U.GetLocalPosY() +
                thunder.th_D.GetLocalPosY()) * 0.5f;
            tf.localPosition = temp;
        }
        { // size
            float deltaY =
                thunder.th_U.GetPosY() -
                thunder.th_D.GetPosY();
            temp = sr.size;
            temp.y = deltaY;
            //
            sr.size = temp;
            //
            temp.x = 2.0f;
            col.size = temp;
        }
    }
    //====================================//
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("FootBoard") == true)
            collision.GetComponent<FootBoard>().DestroyFootBoard();
    }
}
