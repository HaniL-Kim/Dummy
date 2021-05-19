using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inner : MonoBehaviour
{
    //=============================================//
    public Closet closet;
    //=============================================//
    public bool isFliped = false;
    //=============================================//
    public bool isDanger = false;
    public MineTypes mineType = MineTypes.NONE;
    //=============================================//
    public int dangerCount = 0;
    public List<Inner> arroundInners;
    //=============================================//
    // Component
    public SpriteRenderer sr;
    public Animator anim;
    // Anim Hash
    private readonly int hashInnerIsDanger
        = Animator.StringToHash("InnerIsDanger");
    private readonly int hashFlipInner
        = Animator.StringToHash("FlipInner");
    //=============================================//
    // Children
    public Display display;
    public FootBoard footBoard;
    //=============================================//
    //private void Start()
    private void Awake()
    {
        mineType = MineTypes.NONE;
    } // End Start
    //
    private void OnEnable()
    {
        anim.enabled = true;
        anim.SetBool(hashInnerIsDanger, isDanger);
    }
    //=============================================//
    public void ResetInner()
    {
        { // Inner
            isFliped = false;
            mineType = MineTypes.NONE;
            dangerCount = 0;
            // arround
            arroundInners.Clear();
            // anim
            isDanger = false;
            anim.SetBool(hashInnerIsDanger, isDanger);
            anim.SetBool(hashFlipInner, false);
            //
            anim.enabled = false;
            sr.sprite = null;
            //anim.enabled = true;
        }
        // Thunder
        foreach (var item in GetComponentsInChildren<Thunder>())
            item.ResetThunder();
        // Display
        display.ResetDisplay();
        // FootBoard
        footBoard.ResetFootBoard();
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
    public void CheckComplete()
    {
        bool isComplete = true;
        for (int i = 0; i < arroundInners.Count; ++i)
        {
            if (arroundInners[i].isDanger == true)
                if (arroundInners[i].isFliped == false)
                    isComplete =  false;
        }
        //
        if(isComplete)
            FlipArround();
    }
    //
    public void Flip()
    {
        isFliped = true;
        anim.SetBool(hashFlipInner, true);
        //
        if (dangerCount == 0)
            FlipArround();
        /*
        if (isDanger == true)
        { // Danger
            for (int i = 0; i < arroundInners.Count; ++i)
                arroundInners[i].CheckComplete();
        }
        else
        { // Safe
            if (dangerCount == 0)
                FlipArround();
        }
        */
    }
    //
    public void EndFlip()
    { // Flip Anim Frame(10) Event1
        footBoard.Set();
    }
    //
    public void ActivateInner()
    { // Flip Anim Frame(16) Event2
        display.Activate();
        if (display.flagHit == true)
            return;
        //
        if (isDanger)
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
        arroundInners.Clear();
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