using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
//
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ClearMonitor : MonoBehaviour
{
    //==============================//
    // Component
    public SpriteRenderer sr;
    //==============================//
    // Child object
    public Transform leftArm;
    public Transform rightArm;
    //
    public TextMeshPro textMesh;
    //
    public float readyPosY;
    public float clearPosY;
    //==============================//
    void Start()
    {
        InitMonitor();
    }
    //==============================//
    public void SetMonitor(float time, float delay)
    {
        // MovePos
        Tween movePos = transform.DOLocalMoveY(clearPosY, time).SetEase(Ease.OutCubic);
        Tween fadeInColor = sr.DOColor(Color.white, time) ;
        //
        Sequence sq = DOTween.Sequence()
            .AppendInterval(delay)
            .Append(movePos)
            .Join(fadeInColor)
            .AppendInterval(15.0f / 60.0f)
            .AppendCallback(() => { SetTextEnable(); })
            .SetUpdate(true);
            ;
        //
        //Invoke("SetTextEnable", time + delay);
    }
    //
    private void SetTextEnable()
    {
        textMesh.enabled = true;
    }
    //
    public void InitMonitor()
    {
        clearPosY = transform.localPosition.y;
        //
        Vector3 temp = transform.localPosition;
        temp.y += readyPosY;
        transform.localPosition = temp;
        //
        sr.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        //
        textMesh.enabled = false;
    }
    //
    private void SetArms()
    {
        float width = sr.size.x;
        Vector3 temp = Vector3.zero;
        temp.y = 15.5f;
        //
        temp.x = -(width / 2.0f) + 12.5f;
        leftArm.transform.localPosition = temp;
        //
        temp.x = +(width / 2.0f) - 12.5f;
        rightArm.transform.localPosition = temp;
    }
    // ================= Custom Editor ================= //
#if UNITY_EDITOR
    [CustomEditor(typeof(ClearMonitor))]
    public class ShakeButton : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            //
            ClearMonitor obj = (ClearMonitor)target;
            //
            if (GUILayout.Button("SetArms"))
                obj.SetArms();
        }
    }
#endif
}
