using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CheckBoxControl : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject balloon;
    private Button btn;
    //==============================================//
    private void Awake()
    {
        balloon = transform.GetChild(1).gameObject;
        btn = GetComponent<Button>();
    }
    //==============================================//
    public void OnPointerEnter(PointerEventData data)
    {
        if(btn.interactable == false)
            balloon.SetActive(true);
    }
    //
    public void OnPointerExit(PointerEventData data)
    {
        if (btn.interactable == false)
            balloon.SetActive(false);
    }
}
