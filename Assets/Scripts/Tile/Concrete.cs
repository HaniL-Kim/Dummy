using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Concrete : MonoBehaviour
{
    //==================================//
    public bool crashing = false;
    public float crashTime;
    public float crashWaitTime;
    public float crashReturnTime;
    public WaitForSeconds cwt;
    //
    public Vector3 originPos = Vector3.zero;
    public Vector3 targetPos = Vector3.zero;
    //==================================//
    private void Start()
    {
        SetCrashInfo();
    }
    //==================================//
    public void SetCrashInfo()
    {
        crashTime = MineController.instance.crashTime;
        crashWaitTime = MineController.instance.crashWaitTime;
        crashReturnTime = MineController.instance.crashReturnTime;
        cwt = new WaitForSeconds(crashWaitTime);
    }
    //==================================//
    public void StartToMove(Vector3 _targetPos)
    {
        targetPos = _targetPos;
        //
        StartCoroutine(MoveToTarget());
    }
    //
    public IEnumerator MoveToTarget()
    {
        SetOrigin();
        //
        float s = targetPos.x - originPos.x;
        float a = (2.0f * s) / Mathf.Pow(crashTime, 2.0f);
        float v = 0, t = 0;
        //
        while (true)
        {
            if (t >= crashTime)
            {
                SetPosX(targetPos.x);
                //
                yield return cwt;
                //
                StartCoroutine(ReturnToOrigin());
                //
                yield break;
            }
            else
            {
                t += Time.deltaTime;
                v += a * Time.deltaTime;
                //
                AddPosX((v * Time.deltaTime));
            }

            yield return null;
        }
    }
    //
    public IEnumerator ReturnToOrigin()
    {
        float t = 0;
        float speed = 1.0f / crashReturnTime;
        //
        Vector3 startPos = transform.localPosition;
        Vector3 lerpVec = Vector3.zero;
        while (true)
        {
            if (t >= crashReturnTime)
            {
                SetToOrigin();
                //
                yield break;
            }
            else
            {
                t += Time.deltaTime;
                //
                lerpVec = Vector3.Lerp(startPos, originPos, t * speed);
                SetPosX(lerpVec.x);
            }
            //
            yield return null;
        }
    }
    //==================================//
    public void SetToOrigin()
    {
        if(crashing)
        {
            StopAllCoroutines();
            //
            SetPosX(originPos.x);
            crashing = false;
            originPos = Vector3.zero;
            targetPos = Vector3.zero;
        }
    }
    //
    public void SetOrigin()
    {
        crashing = true;
        originPos = transform.localPosition;
    }
    //
    public void SetPosX(float value)
    {
        Vector3 temp = transform.localPosition;
        temp.x = value;
        transform.localPosition = temp;
    }
    //
    public void AddPosX(float value)
    {
        Vector3 temp = transform.localPosition;
        temp.x += value;
        transform.localPosition = temp;
    }
    //==================================//
}
