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
    public int lv = 0;
    //public float speed;
    public int unit;
    public int countOfFloorClimb;
    public float moveDist = 0;
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
        //
        lv = 0;
        SetSpeed(lv);
    }
    //==================================//
    private void Update()
    {
        CountDown();
        SetUIText();
    }
    //==================================//
    public void SetUnit(int value)
    {
        unit = value;
        unitText.text = unit.ToString();
    }
    //==================================//
    public void SetSpeed(int lv)
    {
        ec.elevateSpeed = speeds[lv];
    }
    //
    public void SpeedLVDown()
    {
        // Min LV
        if (lv == 0)
            return;
        --lv;
        SetSpeed(lv);
    }
    //
    public void SpeedLVUp()
    {
        // Max LV
        if (lv == speeds.Count - 1)
            return;
        //
        ++lv;
        SetSpeed(lv);
    }
    //
    public void AddDist(float value)
    {
        moveDist += value;
    }
    //
    private void CountDown()
    {
        countOfFloorClimb = (int)(moveDist / floorHeight);
        //if ((countOfFloorClimb % unit) == 0 && (countOfFloorClimb / unit) != lv)
        if (countOfFloorClimb == unit && lv < speeds.Count)
        {
            moveDist = 0;
            SpeedLVUp();
        }
    }
    //
    public void SetUIText()
    {
        lvText.text = lv.ToString();
        speedText.text = ec.elevateSpeed.ToString();
        unitText.text = unit.ToString();
        countDownText.text = countOfFloorClimb.ToString();
    }
    //==================================//
}
