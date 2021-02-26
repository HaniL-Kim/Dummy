using System.Collections;
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
    //
    public TextMeshProUGUI pressToRetry;
    //============================//
    private void Awake()
    {
        instance = this;
        //
        UpdateRSCText();
        UpdateAirJumpText();
        UpdateShieldText();
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