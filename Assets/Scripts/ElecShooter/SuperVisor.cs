using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SuperVisor : MonoBehaviour
{
    //==================================//
    public const float cfloorHeight = 47.0f;
    //==================================//
    public ElecShooterController ec;
    //
    public List<float> speeds = new List<float>();
    //
    public int lv = 0;
    public int unit;
    public int countOfFloorClimb;
    public float moveDist = 0;
    //
    public int currentFloor = 1;
    private float floorDist = 0;
    //==================================//
    public TextMeshPro countDownText;
    public TextMeshPro lvText;
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
        CurrentFloor();
        CountDown();
        SetUIText();
    }
    //==================================//
    public void CurrentFloor()
    {
        currentFloor = (int)(floorDist / cfloorHeight) + 1;
    }
    //
    public void SetUnit(int value)
    {
        unit = value;
    }
    //
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
        //
        ItemManager.instance.SetItemRate(lv);
        SoundManager.instance.SetBGRPitch(lv);
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
        //
        ItemManager.instance.SetItemRate(lv);
        SoundManager.instance.SetBGRPitch(lv);
        //
        Debug.LogFormat("Speed LV UP to {0}", lv);
    }
    //
    public void AddDist(float value)
    {
        moveDist += value;
        floorDist += value;
    }
    //
    private void CountDown()
    {
        countOfFloorClimb = (int)(moveDist / cfloorHeight);
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
        countDownText.text = (unit - countOfFloorClimb).ToString();
    }
    //==================================//
}
