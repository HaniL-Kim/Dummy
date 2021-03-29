using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CheckBoxControl : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // ====================== Sibling ====================== //
    public CheckBoxControl[] group;
    // ====================== Component ====================== //
    private Button btn;
    // ====================== Children ====================== //
    public GameObject check;
    public GameObject balloon;
    //
    public bool isChecked = false;
    public OptionManager om;
    // ====================== Func : Default ====================== //
    private void Awake()
    {
        btn = GetComponent<Button>();
        //
        check = transform.GetChild(0).gameObject;
        balloon = transform.GetChild(1).gameObject;
        //
        om = FindObjectOfType<OptionManager>(true);
    }
    //
    private void Start()
    {
        group = transform.parent.GetComponentsInChildren<CheckBoxControl>();
    }
    // ====================== Func : Button ====================== //
    public void Check(bool value)
    {
        isChecked = value;
        check.SetActive(value);
    }
    //
    public void CheckToggleInGroup()
    {
        if (group.Length > 1)
        {
            foreach (CheckBoxControl cb in group)
            {
                if (cb == this)
                    Check(!isChecked);
                //
                else
                    cb.Check(false);
            }
        }
        //
        om.SetScreenByChecked();
    }
    //
    public void CheckInGroup()
    {
        if (group.Length > 1)
        {
            foreach (CheckBoxControl cb in group)
            {
                if (cb.isChecked == true)
                    cb.Check(false);
            }
        }
        Check(true);
        //
        om.SetScreenByChecked();
    }
    // ====================== Func : Event ====================== //
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
