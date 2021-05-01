using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TMPControl : MonoBehaviour
{
    //=======================================//
    // Component
    public TextMeshProUGUI tmp;
    //=======================================//
    // Variable
    public string text;
    public List<string> textsToChangeColor;
    public Color colorToChange;
    //=======================================//
    private void Awake()
    {
        tmp = GetComponent<TextMeshProUGUI>();
        text = tmp.text;
        SetColor();
    }
    //=======================================//
    private void SetColor()
    {
        for (int i = 0; i < textsToChangeColor.Count; ++i)
            MyUtility.ChangeStringColor(ref text, textsToChangeColor[i], colorToChange);
            // text = text.Replace(textsToChangeColor[i], ("<color=#ADB5EA>" + textsToChangeColor[i] + "</color>"));
        //
        tmp.SetText(text);
    }
}