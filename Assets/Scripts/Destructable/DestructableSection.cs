using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructableSection : MonoBehaviour
{
    //===============================//
    // Component
    public Rigidbody2D rb;
    public SpriteRenderer sr;
    public BoxCollider2D col;
    private Material mat;
    //===============================//
    // Variable
    public Vector2 forceDir;
    public float torque;
    public float gravity;
    //===============================//
    private readonly int hashDissolveAmount = Shader.PropertyToID("_DissolveAmount");
    public float dissolveSpeed = 1.0f;
    private float dissolveValue = 0;
    //===============================//
    private void Awake()
    {
        mat = GetComponent<Renderer>().material;
    }
    //
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        //
        Gizmos.DrawSphere(GetCOG(), 0.5f);
    }
#endif
    //===============================//
    public Vector3 GetCOG()
    {
        return col.bounds.center;
    }
    //
    public void Shatter()
    {
        rb.gravityScale = gravity;
        rb.AddForce(forceDir);
        rb.AddTorque(torque);
        //
        BeginDissolve();
    }
    //===============================//
    public void BeginDissolve()
    {
        StartCoroutine(DissolveEffect());
    }
    //
    private IEnumerator DissolveEffect()
    {
        while (true)
        {
            mat.SetFloat(hashDissolveAmount, dissolveValue);
            //
            if (dissolveValue < 1.0f)
            {
                dissolveValue += (Time.deltaTime * dissolveSpeed);
                yield return null;
            }
            else
            {
                dissolveValue = 0.0f;
                yield break;
            }
        }
    }
}