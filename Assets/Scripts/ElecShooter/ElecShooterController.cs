using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElecShooterController : MonoBehaviour
{
    //==========================================//
    public enum NarrowLevel
    { NONE, ONE, TWO, THREE }
    //==========================================//
    public GameObject Up;
    public GameObject Down;
    //
    private Vector3 temp = Vector3.zero;
    private Transform tf;
    //==========================================//
    public bool isElevate = false;
    public float elevateSpeed;
    public NarrowLevel level;
    //==========================================//
    private void Awake()
    {
        tf = GetComponent<Transform>();
    }
    //==========================================//
    private void Update()
    {
        Elevate();
        Test();
    }
    //==========================================//
    private void Elevate()
    {
        if (isElevate == false)
            return;
        //
        tf.Translate(Vector3.up * elevateSpeed * Time.deltaTime);
    }
    //
    private void Test()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
            SetLevel(0);
        else if (Input.GetKeyDown(KeyCode.Alpha1))
            SetLevel(1);
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            SetLevel(2);
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            SetLevel(3);
    }
    //
    private void SetLevel(int value)
    {
        level = (NarrowLevel)value;
        {
            temp = Up.transform.position;
            temp.y = 470.0f - (float)level * 47.0f;
            Up.transform.localPosition = temp;
        }
        {
            temp = Down.transform.position;
            temp.y = 000.0f + (float)level * 47.0f;
            Down.transform.localPosition = temp;
        }
    }
    //==========================================//
    //==========================================//
}
