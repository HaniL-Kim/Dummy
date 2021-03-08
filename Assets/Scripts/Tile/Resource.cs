using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    //===============================================//
    public void ResetResource()
    {
        gameObject.SetActive(true);
    }
    //
    public void GainResource()
    {
        gameObject.SetActive(false);
        UIManager.instance.AddRSC();
    }
    //===============================================//
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            GainResource();
        }
    }
    //===============================================//
}
