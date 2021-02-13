using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThunderBody : MonoBehaviour
{
    //=====================================//
    public Thunder thunder;
    //=====================================//
    // components
    private Transform tf;
    private SpriteRenderer sr;
    private Animator anim;
    private BoxCollider2D col;
    //=====================================//
    private readonly int hashBodyExplode =
        Animator.StringToHash("ThunderExplode");
    //=====================================//
    Vector3 temp = Vector3.zero;
    //====================================//
    private void Awake()
    {
        thunder = transform.GetComponentInParent<Thunder>();
        //
        tf = GetComponent<Transform>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        col = GetComponent<BoxCollider2D>();
    }
    //=====================================//
    public void Reset()
    {
        if (thunder == null)
            return;
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
        thunder.Reset();
    }
    //
    public void Explode()
    {
        col.enabled = true;
        anim.SetTrigger(hashBodyExplode);
    }
    //
    public void SetBody()
    {
        // pos
        {
            temp = Vector3.zero;
            temp.y = (thunder.head_U.localPosition.y
                + thunder.head_D.localPosition.y) * 0.5f;
            tf.localPosition = temp;
        }
        // size
        {
            float deltaY = thunder.head_U.position.y
                - thunder.head_D.position.y;
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
