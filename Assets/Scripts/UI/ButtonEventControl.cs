using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonEventControl : MonoBehaviour,
    IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler
{
    Button btn;
    //========== Func : Default ==========//
    private void Awake()
    {
        btn = GetComponent<Button>();
    }
    //========== Func : EventTrigger ==========//
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (btn.IsInteractable())
            SoundManager.instance.Play(SoundKey.MOUSE_ON);
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (btn.IsInteractable())
            SoundManager.instance.Play(SoundKey.MOUSE_BTN_DOWN);
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (btn.IsInteractable())
            SoundManager.instance.Play(SoundKey.MOUSE_BTN_UP);
    }
}
