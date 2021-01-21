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
    private void Awake()
    {
        instance = this;
    }
    //
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
            SoundManager.instance.PlayBGM(SoundKey.BGM);
    }
    //
}