using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SuperVisor : MonoBehaviour
{
    //==================================//
    public const float floorHeight = 47.0f;
    //==================================//
    public ElecShooterController ec;
    //
    public List<float> speeds = new List<float>();
    //
    public int lv = -1;
    public float speed = 12;
    public int unit = 25;
    public int countDown = 0;
    //==================================//
    public TextMeshPro lvText;
    public TextMeshPro speedText;
    public TextMeshPro unitText;
    public TextMeshPro countDownText;
    //==================================//
    public Vector3 startPos;
    //==================================//
    private void Awake()
    {
        startPos = transform.position;
    }
    //==================================//
    private void Update()
    {
        CountDown();
        SetUIText();
    }
    //==================================//
    private void SpeedUp()
    {
        if (lv == speeds.Count-1)
            return;
        //
        speed = speeds[++lv];
        ec.elevateSpeed = speed;
    }
    //
    private void CountDown()
    {
        countDown = (int)((transform.position.y - startPos.y) / floorHeight);
        if ((countDown % unit) == 0 && (countDown / unit) != lv)
            SpeedUp();
    }
    //
    public void SetUIText()
    {
        speed = ec.elevateSpeed;
        //
        lvText.text = lv.ToString();
        speedText.text = speed.ToString();
        unitText.text = unit.ToString();
        countDownText.text = countDown.ToString();
    }
    //==================================//
}
