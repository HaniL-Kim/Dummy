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
    public Animator anim;
    //=============================================//
    public Display display;
    public GameObject footBoard;
    //
    private readonly int hashInnerIsDanger
        = Animator.StringToHash("InnerIsDanger");
    //=============================================//
    //private void Start()
    private void Awake()
    {
        mineType = MineTypes.NONE;
    } // End Start
    //
    private void OnEnable()
    {
        anim.SetBool(hashInnerIsDanger, isDanger);
    }
    //=============================================//
    public Floor GetFloor()
    {
        Transform floorTF = transform.parent.parent;
        Transform stageTF = floorTF.parent;
        //
        int floorNum = floorTF.name[0] - '0';
        int stageNum = stageTF.name[0] - '0';
        //
        Floor floor = Map.instance.GetFloor(stageNum, floorNum);
        //
        return floor;
    }
    //=============================================//
    public void FlipCloset()
    {
        closet.Flip();
    }
    //
    public void FlipSides()
    {
        foreach (Inner temp in arroundInners)
        {
            float distY = Mathf.Abs(temp.transform.position.y - transform.position.y);
            if (distY < 0.1f)
                temp.FlipCloset();
        }
    }
    //
    public void FlipArround(bool isSafe = true)
    {
        foreach (Inner temp in arroundInners)
        {
            if (isSafe)
            { // Call From DangerCount 0
                if (temp.isDanger == false)
                    temp.FlipCloset();
            }
            else
            { // Call From HardMode Mine
                temp.FlipCloset();
            }
        }
    }
    //
    public void Flip()
    {
        anim.SetBool("Flip", true);
        //
        if (dangerCount == 0 && isDanger == false)
            FlipArround();
    }
    //
    public void EndFlip()
    { // Flip Anim Frame(10) Event1
        footBoard.GetComponent<FootBoard>().Set();
    }
    //
    public void ActivateInner()
    { // Flip Anim Frame(16) Event2
        display.Activate();
        if (display.flagHit == true)
            return;
        //
        MineController.instance.BeginAlert(mineType, this);
    } // End ShowDisplay()

    //=============================================//
    public void SetDanger(int value)
    { // value : 0 ~ 5
        //GameManager.instance.Loading(); // MineCount
        mineType = (MineTypes)(value);
        //
        isDanger = true;
        anim.SetBool(hashInnerIsDanger, isDanger);
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
                arroundInners.Add(hit.collider.GetComponent<Inner>());
                //
                if (hit.collider.GetComponent<Inner>().isDanger == true)
                    ++dangerCount;
                //else
            }
        }
        if(isDanger == false)
            SetNumber(dangerCount);
        //
        GetComponent<CircleCollider2D>().enabled = true;
    } // End CheckArround()
    //

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    for(int i = 0; i < dirs.Count; ++i)
    //        Gizmos.DrawRay(transform.position, dirs[i]);
    //}
    //
    //=============================================//
}