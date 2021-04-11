using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Column : MonoBehaviour
{
    // ====================================== //
    public const float cfloorHeight = 47.0f;
    // ====================================== //
    public Transform block_above;
    public Transform block_below;
    //
    public TextMeshPro text_above;
    public TextMeshPro text_below;
    // ====================================== //
    public SuperVisor sv;
    // ================= Func : Default ================= //
    public void Awake()
    {
        block_above = transform.GetChild(0);
        block_below = transform.GetChild(1);
        //
        text_above = block_above.GetComponentInChildren<TextMeshPro>();
        text_below = block_below.GetComponentInChildren<TextMeshPro>();
    }
    //
    public void Start()
    {
        FindSupervisor();
        SetFloor();
    }
    //
    public void OnEnable()
    {
        FindSupervisor();
        SetFloor();
    }
    //
    private void Update()
    {
        //SetFloor();
    }
    // ================= Func ================= //
    private void FindSupervisor()
    {
        if (sv == null)
            sv = FindObjectOfType<SuperVisor>(true);
        else
            return;
    }
    //
    private int CalcFloor()
    {
        float distY = sv.transform.position.y - block_below .transform.position.y;
        int thisFloor = sv.currentFloor - Mathf.CeilToInt(distY / cfloorHeight) + 1;
        //
        return thisFloor;
    }
    //
    public void SetFloor()
    {
        int floorNumBelow = CalcFloor();
        SetText(floorNumBelow);
    }
    //
    public void SetText(string s)
    {
        text_below.text = s;
        text_above.text = s;
    }
    //
    public void SetText(int belowNum)
    {
        if (belowNum == -1)
        {
            text_below.text = "Base";
            text_above.text = "Base";
        }
        else
        {
            text_below.text = belowNum.ToString() + "F";
            text_above.text = (belowNum + 1).ToString() + "F";
        }
    }
}
