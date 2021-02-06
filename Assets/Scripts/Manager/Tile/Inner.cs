using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inner : MonoBehaviour
{
    //=============================================//
    public int dangerCount = 0;
    public bool isDanger = false;
    public MineTypes mineType = MineTypes.NONE;
    //=============================================//
    public Closet closet;
    //=============================================//
    public List<Inner> arroundInners;
    //=============================================//
    private Animator anim;
    //=============================================//
    private Display display;
    private GameObject footBoard;
    //=============================================//
    //private void Start()
    private void Awake()
    {
        mineType = MineTypes.NONE;
        anim = GetComponent<Animator>();
        //
        display = transform.GetChild(0).GetComponent<Display>();
        display.inner = this;
        //
        footBoard = transform.GetChild(1).gameObject;
    } // End Start
    //=============================================//
    private void FlipCloset()
    {
        closet.Flip();
    }
    private void FlipArround()
    {
        foreach (Inner temp in arroundInners)
        {
            temp.FlipCloset();
        }
    } // End FlipArround()
    public void Flip()
    {
        anim.SetBool("Flip", true);
        //
        if (dangerCount == 0)
            FlipArround();
    } // End Flip()
    public void EndFlip()
    { // Flip Anim Frame(10) Event1
        //footBoard.GetComponent<FootBoard>().Set();
    } // End EndFlip()
    public void ActivateInner()
    { // Flip Anim Frame(16) Event2
        footBoard.GetComponent<FootBoard>().Set();
        //
        display.Activate();
        //
        switch (mineType)
        {
            case MineTypes.NONE:
                break;
            case MineTypes.PULL:
                EffectManager.instance.Play("Pull", transform);
                break;
            case MineTypes.PUSH:
                EffectManager.instance.Play("Push", transform);
                break;
            case MineTypes.NARROWING:
                break;
            case MineTypes.CRASH:
                break;
            case MineTypes.GHOST:
                break;
            case MineTypes.LIGHTNING:
                break;
            default:
                break;
        }
    } // End ShowDisplay()

    //=============================================//
    public void SetDanger(int value)
    { // value : 0 ~ 5
        mineType = (MineTypes)(value);
        //
        anim.SetBool("Danger", (isDanger = true));
        //
        display.SetDanger(isDanger, value);
    } // End SetDanger()

    public void SetNumber(int value)
    { // value : 0 ~ 6
        display.SetNumber(value);
    } // End SetDanger()
    
    public void CheckArround()
    {
        // avoid Self Collision
        GetComponent<CircleCollider2D>().enabled = false;
        // 
        dangerCount = 0;
        for(int i = 0; i < 6; ++i)
        {
            RaycastHit2D hit = GameManager.instance.RayToDirs(
                transform, i, GameManager.instance.innerLayer);

            if (hit)
            {
                if (hit.collider.GetComponent<Inner>().isDanger == true)
                    ++dangerCount;
                else
                    arroundInners.Add(hit.collider.GetComponent<Inner>());
            }
        }
        SetNumber(dangerCount);
        GetComponent<CircleCollider2D>().enabled = true;
    } // End CheckArround()

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    for(int i = 0; i < dirs.Count; ++i)
    //        Gizmos.DrawRay(transform.position, dirs[i]);
    //}
    //
    //=============================================//
}