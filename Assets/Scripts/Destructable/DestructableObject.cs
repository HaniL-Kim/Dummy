using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//
#if UNITY_EDITOR
using UnityEditor;
#endif

public class DestructableObject : MonoBehaviour
{
    //==============================================//
    public List<DestructableSection> sections;
    //==============================================//
    public bool destroyAtStart = false; // Debug
    //
    public Transform spc;
    public float scatterPower;
    public float spinSpeed;
    public float sectionGravity;
    public float disappearSpeed;
    //
    public bool randomSPCMode = false;
    public Vector2 randomSPCRangeMin;
    public Vector2 randomSPCRangeMax;
    //==============================================//
    void Start()
    {
        if(destroyAtStart)
            SetShatter();
    }
    //==============================================//
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //
        Gizmos.DrawSphere(spc.position, 1.0f);
        //
        Gizmos.color = Color.green;
        for (int i = 0; i < sections.Count; ++i)
        {
            if (sections[i] == null)
                return;
            //
            Vector3 secPos = sections[i].GetCOG();
            Vector2 dir = secPos - spc.transform.position;
            Gizmos.DrawRay(spc.transform.position, dir);
        }
    }
#endif
    //==============================================//
    public void Flip(bool b)
    {
        for (int i = 0; i < sections.Count; ++i)
            sections[i].sr.flipX = b;
    }
    //
    public void DestroySelf()
    {
        Destroy(this.gameObject);
    }
    //
    public void SetShatter()
    {
        if(randomSPCMode == true)
        {
            // spc
            Vector3 temp = Vector3.zero;
            temp.x = Random.Range(randomSPCRangeMin.x, randomSPCRangeMax.x);
            temp.y = Random.Range(randomSPCRangeMin.y, randomSPCRangeMax.y);
            spc.transform.localPosition = temp;
            // sp
            scatterPower += Random.Range(-20.0f, 20.0f);
            // ss
            int plusminus = (Random.Range(0, 2) * 2) - 1;
            spinSpeed += Random.Range(-20.0f, 20.0f);
            spinSpeed *= (float)plusminus;
        }
        //
        for (int i = 0; i < sections.Count; ++i)
        {
            Vector3 secPos = sections[i].GetCOG();
            Vector2 dir = secPos - spc.transform.position;
            //
            sections[i].forceDir = dir * scatterPower * 10.0f;
            sections[i].torque = spinSpeed * 100.0f;
            sections[i].gravity = sectionGravity;
            sections[i].dissolveSpeed = disappearSpeed;
            //
            sections[i].Shatter();
        }
        //
        Invoke("DestroySelf", 5.0f);
    }
    //==============================================//
    // ================= Custom Editor ================= //
#if UNITY_EDITOR
    [CustomEditor(typeof(DestructableObject))]
    public class DestroyButton : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            //
            DestructableObject sc = (DestructableObject)target;
            //
            if (GUILayout.Button("Destroy"))
                sc.SetShatter();
        }
    }
#endif
}
