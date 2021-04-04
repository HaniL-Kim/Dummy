﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    //============================//
    public static UIManager instance;
    //============================//
    public Dummy dummy;
    //============================//
    public int rscCount = 0;
    public TextMeshProUGUI rscUI;
    public int airJumpCount = 0;
    public TextMeshProUGUI airJumpUI;
    public int shieldCount = 0;
    public TextMeshProUGUI shieldUI;
    // Retry
    public TextMeshProUGUI pressToRetry;
    // Exit
    public Image exitPannel;
    //
    public GameObject option;
    //============================//
    private void Awake()
    {
        instance = this;
        //
        UpdateRSCText();
        UpdateAirJumpText();
        UpdateShieldText();
        //
        //SetOption();
    }
    //============================//
    private void Update()
    {
        PauseControl();
    }
    //============================//
    //private void SetOption()
    //{
    //    option = GameObject.FindGameObjectWithTag("OptionWindow");
    //}
    //
    public void ExitGame()
    { // Button Event
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBPLAYER
        Application.OpenURL("http://google.com");
#else
        Application.Quit(); //어플리케이션 종료
#endif
    }
    //
    private void PauseControl()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            bool isActive = SceneControl.instance.IGMEnabled();
            //
            SceneControl.instance.SetIGM(!isActive);
        }
    }
    //
    public void SetPause(bool b)
    {
        Time.timeScale = b ? 0.0f : 1.0f;
        GameManager.instance.pause = b;
    }
    //============================//
    public bool UseRSC(int cost)
    {
        if (rscCount - cost >= 0)
        {
            rscCount -= cost;
            UpdateRSCText();
            return true;
        }
        else
            return false;
    }
    //
    public void AddRSC(int value = 1)
    {
        rscCount += value;
        UpdateRSCText();
    }
    //============================//
    public void ActiveShield()
    {
        --shieldCount;
        UpdateShieldText();
    }
    //
    public void AddShield()
    {
        ++shieldCount;
        UpdateShieldText();
        //
        if(dummy.shieldState == ShieldEffect.ShieldState.NONE)
            dummy.Outline(ShieldEffect.ShieldState.NORMAL);
    }
    //
    public void UseAirJump()
    {
        airJumpCount--;
        UpdateAirJumpText();
        //
        if(airJumpCount == 0)
            dummy.arm.EquipJumpPack(false);
    }
    //
    public void AddAJPack()
    {
        if (airJumpCount == 0)
        {
            AirJumpPack jumpPack = ItemManager.instance.GetJumpPack();
            if (MyUtility.IsNull(jumpPack, "JumpPack"))
                return;
            //
            dummy.arm.EquipJumpPack(true, jumpPack);
        }
        //
        ++airJumpCount;
        UpdateAirJumpText();
    }
    //============================//
    private void UpdateShieldText()
    {
        shieldUI.text = shieldCount.ToString();
    }
    private void UpdateAirJumpText()
    {
        airJumpUI.text = airJumpCount.ToString();
    }
    private void UpdateRSCText()
    {
        rscUI.text = rscCount.ToString();
    }
    //============================//
    public void ActivatePressToRetry()
    {
        pressToRetry.gameObject.SetActive(true);
    }
    //public void SetSliderValue(float value)
    //{
    //    if (loadingSlider.gameObject.activeSelf == false)
    //        loadingSlider.gameObject.SetActive(true);
    //    //
    //    loadingSlider.value = value;
    //}
}