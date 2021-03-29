using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InfoScreenControl : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    //==============================================//
    public GameObject frame;
    private Image img;
    //==============================================//
    private void Awake()
    {
        frame = GameObject.FindGameObjectWithTag("FrameScreen");
        img = frame.GetComponent<Image>();
    }
    //==============================================//
    public void OnPointerEnter(PointerEventData data)
    {
        if(data.pointerEnter != null)
            img.sprite = data.pointerEnter.GetComponent<Image>().sprite;
    }
    //
    public void OnPointerExit(PointerEventData data)
    {
        img.sprite = null;
    }
    //==============================================//
}
