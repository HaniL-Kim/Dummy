using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldEffect : MonoBehaviour
{
    //==================================//
    public enum ShieldState { NONE, NORMAL, ACTIVE };
    //==================================//
    public Dummy dummy;
    //==================================//
    [SerializeField]
    private int curPlayCount = 0;
    public int endCount = 3;
    //==================================//
    // Animator Event
    public void Activate()
    {
        gameObject.SetActive(true);
    }
    //
    public void CheckPlayCount()
    {
        curPlayCount++;
        if (curPlayCount == endCount)
        {
            curPlayCount = 0;
            //
            if(UIManager.instance.shieldCount <= 0)
                dummy.Outline(ShieldState.NONE);
            else
                dummy.Outline(ShieldState.NORMAL);
            //
            gameObject.SetActive(false);
        }
    }
}
