using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
//
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ClearControl : MonoBehaviour
{
    //==============================//
    public List<ClearMonitor> clearMonitors;
    public SpriteRenderer flash;
    public FollowTarget cam;
    public GameObject pressTo;
    //
    public float flashBeginTime;
    public float flashEndTime;
    public float monitorDelayTime;
    //==============================//
    public void ActivatePressTo(bool b)
    {
        pressTo.gameObject.SetActive(b);
    }
    //
    public void SetControlPos(float posY)
    {
        Vector3 temp = transform.position;
        temp.y = posY + 30.0f;
        transform.position = temp;
    }
    //
    public void ResetMonitor()
    {
        DOTween.Kill("ClearControlSequence");
        //
        foreach (ClearMonitor monitor in clearMonitors)
            monitor.InitMonitor();
        //
        ActivatePressTo(false);
    }
    //
    public void Activate()
    {
        // Debug
        Time.timeScale = 0;
        //
        int monitorCount = 3;
        if (SceneControl.instance != null)
            if (SceneControl.instance.currentStage == 6)
                monitorCount = 4;
        //
        Tween tween_FlashBegin
            = flash.DOColor(new Color(1, 1, 1, 0.5f), flashBeginTime)
            .SetUpdate(true); // 5 frame
        Tween tween_FlashEnd
            = flash.DOColor(new Color(1, 1, 1, 0), flashEndTime)
            .SetUpdate(true); // 25 frame
        TweenCallback tcb_CamToCenter
            = cam.MoveToTarget(flashBeginTime + flashEndTime); // 30 frame
        TweenCallback tcb_SetMonitor
            = () =>
            {
                float delay = 0;
                //foreach (ClearMonitor monitor in clearMonitors)
                for(int i = 0; i < monitorCount; ++i)
                {
                    clearMonitors[i].SetMonitor(flashBeginTime + flashEndTime, delay);
                    delay += monitorDelayTime; // + 40 frame
                }
            };
        //
        Sequence sq = DOTween.Sequence()
            .Append(tween_FlashBegin)
            .Append(tween_FlashEnd)
            .AppendCallback(tcb_CamToCenter)
            .AppendInterval(flashBeginTime + flashEndTime)
            .AppendCallback(tcb_SetMonitor)
            .AppendInterval(monitorDelayTime * monitorCount)
            .AppendCallback(() => { ActivatePressTo(true); })
            .SetUpdate(true)
            .SetId("ClearControlSequence")
            ;

    }
    // ================= Custom Editor ================= //
#if UNITY_EDITOR
    [CustomEditor(typeof(ClearControl))]
    public class ShakeButton : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            //
            ClearControl obj = (ClearControl)target;
            //
            if (GUILayout.Button("Activate"))
                obj.Activate();
            if (GUILayout.Button("ResetMonitor"))
                obj.ResetMonitor();
        }
    }
#endif
}