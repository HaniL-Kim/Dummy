using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inner : MonoBehaviour
{
    //=============================================//
    public bool isDanger = false;
    //
    List<Vector3> dirs = new List<Vector3>();
    private int innerLayer = 0;
    //=============================================//
    //
    private Animator anim;
    private GameObject display;
    private GameObject footBoard;
    //=============================================//
    //private void Start()
    private void Awake()
    {
        anim = GetComponent<Animator>();
        display = transform.GetChild(0).gameObject;
        footBoard = transform.GetChild(1).gameObject;
        //
        SetDirs();
    } // End Start
    //=============================================//
    void SetDirs()
    {
        dirs.Add(new Vector3(+64.0f, 0, 0));
        dirs.Add(new Vector3(+32.0f, +47.0f, 0));
        dirs.Add(new Vector3(-32.0f, +47.0f, 0));
        dirs.Add(new Vector3(-64.0f, 0, 0));
        dirs.Add(new Vector3(-32.0f, -47.0f, 0));
        dirs.Add(new Vector3(+32.0f, -47.0f, 0));
        //
        innerLayer = LayerMask.GetMask("Inner");
    } // End SetDirs()

    public void Flip()
    {
        anim.SetBool("Flip", true);
    } // End Flip()
    public void EndFlip()
    { // Flip Anim Frame(10) Event1
        footBoard.GetComponent<FootBoard>().Set();
    } // End EndFlip()
    public void ShowDisplay()
    { // Flip Anim Frame(16) Event2
        //display.SetActive(true);
        display.GetComponent<SpriteRenderer>().enabled = true;
    } // End ShowDisplay()

    //=============================================//
    public void SetDanger(int value)
    { // value : 7 ~ 12
        anim.SetBool("Danger", (isDanger = true));
        display.GetComponent<SpriteRenderer>().sprite =
            GameManager.instance.displayTextures[value].texture;
    } // End SetDanger()

    public void SetNumber(int value)
    { // value : 0 ~ 6
        //anim.SetBool("Danger", (isDanger = false));
        display.GetComponent<SpriteRenderer>().sprite =
            GameManager.instance.displayTextures[value].texture;
    } // End SetDanger()
    
    public void CheckArround()
    {
        GetComponent<CircleCollider2D>().enabled = false;
        int count = 0;
        for(int i = 0; i < dirs.Count; ++i)
        {
            float dist = dirs[i].magnitude;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dirs[i], dist, innerLayer);

            if (hit)
            {
                if (hit.collider.GetComponent<Inner>().isDanger == true)
                    ++count;
            }
        }
        SetNumber(count);
        GetComponent<CircleCollider2D>().enabled = true;
    } // End CheckArround()

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        for(int i = 0; i < dirs.Count; ++i)
            Gizmos.DrawRay(transform.position, dirs[i]);
    }
    //
    //=============================================//
}