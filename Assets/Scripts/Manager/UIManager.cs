using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    //============================//
    public static UIManager instance;
    //============================//
    public int dashCount = 0;
    public TextMeshProUGUI dashCountUI;
    //============================//
    private void Awake()
    {
        instance = this;
    }
    //
    private void Update()
    {
        dashCountUI.text = dashCount.ToString();
    }
    //============================//
}
