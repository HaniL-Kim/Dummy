using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Shaker : MonoBehaviour
{
    // Shake
    public bool allowRotate = true;
    //
    public bool isShake = false;
    //
    private Vector3 originPosition;
    private Quaternion originRotation;
    //
    private float shake_decay, shake_intensity, shake_time;
    public float decay = 0.002f, intensity = 0.3f, time = 0.33f;
    //=============================================//
    private void Update()
    {
        Shake();
    }
    //=============================================//
    private void ResetShake()
    {
        isShake = false;
        shake_decay = decay;
        shake_intensity = intensity;
        shake_time = time;
        //
        transform.position = originPosition;
        transform.rotation = originRotation;
    }
    //
    private void Shake()
    {
        if (isShake == false)
            return;
        //
        if (shake_time > 0)
        {
            transform.position = originPosition + Random.insideUnitSphere * shake_intensity;
            //
            if (allowRotate == true)
            {
                transform.rotation = new Quaternion(
                originRotation.x + Random.Range(-shake_intensity, shake_intensity) * .2f,
                originRotation.y + Random.Range(-shake_intensity, shake_intensity) * .2f,
                originRotation.z + Random.Range(-shake_intensity, shake_intensity) * .2f,
                originRotation.w + Random.Range(-shake_intensity, shake_intensity) * .2f
                );
            }
            //
            shake_intensity -= shake_decay;
            shake_time -= Time.deltaTime;
        }
        else
        {
            isShake = false;
            ResetShake();
        }
    }
    //
    public void StartShake(float shakeTime = 0, float fadeTime = 0)
    {
        isShake = true;
        originPosition = transform.position;
        originRotation = transform.rotation;
        //
        if (shakeTime != 0)
            time = shakeTime;
    }
    // ================= Custom Editor ================= //
#if UNITY_EDITOR
    [CustomEditor(typeof(Shaker))]
    public class ShakeButton : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            //
            Shaker obj = (Shaker)target;
            //
            if (GUILayout.Button("Shake"))
                obj.StartShake();
        }
    }
#endif
}