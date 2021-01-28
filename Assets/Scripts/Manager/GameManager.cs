using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==================// //==================//
public enum DisplayIcons
{
    ZERO, ONE, TWO, THREE, FOUR, FIVE, SIX,
    ATTRACTION, REPULSION, GHOST, LIGHTNING, NARROWING, CONCRETE
}
//==================// //==================//
[System.Serializable]
public struct DisplayTexture
{
    public DisplayIcons key;
    public Sprite texture;
}
//==================// //==================//
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    //
    public DisplayTexture[] displayTextures;
    //
    //====================================//
    // layer hashing
    public int footBoardLayer;
    public int concreteLayer;
    public int closetLayer;
    //====================================//
    private void Awake()
    {
        instance = this;
        CreateLayerHash();
    }
    //
    private void Start()
    {
        EffectManager.instance.CreateEffect();
    }
    //
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
            SoundManager.instance.PlayBGM(SoundKey.BGM);
    }
    //
    private void CreateLayerHash()
    {
        footBoardLayer = LayerMask.GetMask("FootBoard");
        concreteLayer = LayerMask.GetMask("Concrete");
        closetLayer = LayerMask.GetMask("Closet");
    }
    //
    public Transform GetTileTransformOnMouse()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 dirMouseToForward = new Vector3(mousePos.x, mousePos.y, 20.0f);
        //
        RaycastHit2D hit = Physics2D.Raycast(
            mousePos, dirMouseToForward, 1.0f, closetLayer);
        //
        if (hit)
            return hit.transform;
        else
            return null;
    }
}