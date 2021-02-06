using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootBoard : MonoBehaviour
{
    //=============================================//
    public bool canDistroy = false;
    //=============================================//
    private readonly int hashDissolveAmount = Shader.PropertyToID("_DissolveAmount");
    public float dissolveSpeed = 1.0f;
    private float dissolveValue = 0;
    //=============================================//
    private SpriteRenderer sr;
    private Collider2D col;
    private Material mat;
    //=============================================//
    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        mat = GetComponent<Renderer>().material;
    }
    //=============================================//
    public void Set()
    {
        sr.enabled = true;
        canDistroy = true;
    }
    public void DestroyFootBoard()
    {
        if (canDistroy == false)
            return;
        //
        canDistroy = false;
        col.enabled = false;
        // Dissolve
        StartCoroutine(DissolveEffect());
    }
    private IEnumerator DissolveEffect()
    {
        while (true)
        {
            mat.SetFloat(hashDissolveAmount, dissolveValue);
            //
            if (dissolveValue < 1.0f)
            {
                dissolveValue += Time.deltaTime * dissolveSpeed;
                yield return null;
            }
            else
            {
                dissolveValue = 0.0f;
                yield break;
            }
        }
    }
    //=============================================//
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Concrete"))
            DestroyFootBoard();
    }
    //=============================================//
}
