using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
//
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SuperVisor : MonoBehaviour
{
    //==================================//
    public const float cfloorHeight = 47.0f;
    //==================================//
    public ElecShooterController ec;
    //
    public Shaker shaker;
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
    //
    public GameObject monitor;
    public SpriteRenderer Monitor_CDD_SR;
    public GameObject svBody;
    public SpriteRenderer svBody_LVUP_SR;
    //
    public Color fadeColor;
    //==================================//
    [ColorUsageAttribute(true, true)]
    public Color outlineColor_LVUp;
    public float outlineThickness_LVUp;
    [ColorUsageAttribute(true, true)]
    public Color outlineColor_LVDown;
    public float outlineThickness_LVDown;
    //==================================//
    private void Awake()
    {
        monitor = transform.GetChild(0).gameObject;
        Monitor_CDD_SR = monitor.transform.GetChild(0).GetComponent<SpriteRenderer>();
        //
        svBody = transform.GetChild(1).gameObject;
        svBody_LVUP_SR = svBody.transform.GetChild(0).GetComponent<SpriteRenderer>();
        //
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
        moveDist = 0;
        // Min LV
        if (lv == 0)
            return;
        --lv;
        SetSpeed(lv);
        //
        ItemManager.instance.SetItemRate(lv);
        SoundManager.instance.SetBGRPitch(lv);
        //
        SpeedLVDownEffect();
        //
        Debug.LogFormat("Speed Down to LV{0}", lv);
    }
    //
    public void SpeedLVUp()
    {
        moveDist = 0;
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
        SpeedLVUpEffect();
        //
        Debug.LogFormat("Speed UP to LV{0}", lv);
    }
    //
    public void AddDist(float value)
    {
        moveDist += value;
        floorDist += value;
    }
    //
    private void SpeedLVDownEffect()
    {
        DOTween.Kill("SpeedUP_Eff");
        //
        TweenCallback tcb_StartShake =
            () => { shaker.StartShake(); };
            //() => { shaker.StartShake(30.0f / 60.0f, 15.0f / 60.0f); };
        //
        TweenCallback tcb_SetOutline =
            () =>
            {
                foreach (var ol in GetComponentsInChildren<Outline>())
                    ol.SetOutline(outlineColor_LVDown, outlineThickness_LVDown);
            };
        //
        TweenCallback tcb_FadeOutline =
            () =>
            {
                foreach (var ol in GetComponentsInChildren<Outline>())
                    ol.FadeOutline(fadeColor, 15.0f / 60.0f);
            };
        //
        Sequence sq = DOTween.Sequence()
            .AppendCallback(tcb_SetOutline)
            .AppendCallback(tcb_StartShake)
            .AppendInterval(30.0f / 60.0f)
            .AppendCallback(tcb_FadeOutline)
            .SetId("SpeedDOWN_Eff")
            ;
        //
    }
    //
    private void SpeedLVUpEffect()
    {
        DOTween.Kill("SpeedDOWN_Eff");
        //
        TweenCallback tcb_SetColor = () => { svBody_LVUP_SR.color = Color.white; };
        Tween tween_Fade = svBody_LVUP_SR.DOColor(fadeColor, 15.0f / 60.0f);
        //
        TweenCallback tcb_SetOutline = () => {
            foreach (var ol in GetComponentsInChildren<Outline>())
                ol.SetOutline(outlineColor_LVUp, outlineThickness_LVUp);
        };
        TweenCallback tcb_FadeOutline = () => {
            foreach (var ol in GetComponentsInChildren<Outline>())
                ol.FadeOutline(fadeColor, 15.0f / 60.0f);
        };
        //
        Sequence sq = DOTween.Sequence()
            .AppendCallback(tcb_SetColor)
            .AppendCallback(tcb_SetOutline)
            .AppendInterval(30.0f / 60.0f)
            .AppendCallback(tcb_FadeOutline)
            .Join(tween_Fade)
            .SetId("SpeedUP_Eff")
            ;
        //
    }
    //
    private void MonitorCountDownEffect()
    {
        TweenCallback tcb_SetColor = () => { Monitor_CDD_SR.color = Color.white; };
        Tween tween_Fade = Monitor_CDD_SR.DOColor(fadeColor, 10.0f / 60.0f);
        Sequence sq = DOTween.Sequence()
            .AppendCallback(tcb_SetColor)
            .AppendInterval(10.0f / 60.0f)
            .Append(tween_Fade)
            ;
    }
    //
    private void CountDown()
    {
        countOfFloorClimb = (int)(moveDist / cfloorHeight);
        //if ((countOfFloorClimb % unit) == 0 && (countOfFloorClimb / unit) != lv)
        if (countOfFloorClimb == unit && lv < speeds.Count - 1)
        {
            SpeedLVUp();
        }
    }
    //
    public void SetUIText()
    {
        if (lv == 5)
        {
            lvText.text = "MAX";
            countDownText.text = "";
            return;
        }
        else
        {
            lvText.text = lv.ToString();

            if (countDownText.text != (unit - countOfFloorClimb).ToString())
                MonitorCountDownEffect();
            //
            countDownText.text = (unit - countOfFloorClimb).ToString();
            return;
        }
    }
    // ================= Custom Editor ================= //
#if UNITY_EDITOR
    [CustomEditor(typeof(SuperVisor))]
    public class SuperVisorButton : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            //
            SuperVisor obj = (SuperVisor)target;
            //
            if (GUILayout.Button("SpeedLVDownEffect"))
                obj.SpeedLVDownEffect();
            if (GUILayout.Button("SpeedLVUpEffect"))
                obj.SpeedLVUpEffect();
        }
    }
#endif
}
