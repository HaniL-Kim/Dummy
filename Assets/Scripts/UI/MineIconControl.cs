using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MineIconControl : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject frame;
    private RectTransform content;
    private RectTransform rtf;
    private Vector3 frameDefaultPos = new Vector3(0, 184.0f, 0);
    //==============================================//
    private void Awake()
    {
        content = transform.parent.GetComponent<RectTransform>();
        rtf = GetComponent<RectTransform>();
    }
    //==============================================//
    public void OnPointerEnter(PointerEventData data)
    {
        //Vector3 pos = frame.GetComponent<RectTransform>().position;
        //pos.x = rtf.position.x;
        //frame.GetComponent<RectTransform>().position = pos;
        ////
        //frame.SetActive(true);
    }
    //
    public void OnPointerExit(PointerEventData data)
    {
        //frame.SetActive(false);
        //frame.GetComponent<RectTransform>().anchoredPosition = frameDefaultPos;
    }
}
