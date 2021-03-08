using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootBoardTigger : MonoBehaviour
{
    private FootBoard fb;
    //=============================================//
    private void Start()
    {
        fb = GetComponentInParent<FootBoard>();
    }
    //=============================================//
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Concrete"))
        {
            if (col is BoxCollider2D)
            {
                if (fb)
                    fb.DestroyFootBoard();
            }
        }
    }
}
