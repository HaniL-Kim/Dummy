using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==================// //==================//
public enum MineTypes
{
    NONE = -1,
    PULL, PUSH, NARROWING, CRASH, GHOST, THUNDER
}
//
public enum NumberIcons
{
    NONE = -1,
    ZERO, ONE, TWO, THREE, FOUR, FIVE, SIX,
}
//==================// //==================//
[System.Serializable]
public struct DisplayTexture
{
    public MineTypes mineType;
    public NumberIcons number;
    public Sprite texture;
}
//==================// //==================//
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    //
    public Transform player;
    public ElecShooterController elecShooter;
    //
    public DisplayTexture[] mineIcons;
    public DisplayTexture[] numIcons;
    //====================================//
    // layer hashing
    [HideInInspector] public int footBoardLayer;
    [HideInInspector] public int concreteLayer;
    [HideInInspector] public int closetLayer;
    [HideInInspector] public int innerLayer;
    //
    public List<Vector3> dirs = new List<Vector3>();
    //====================================//
    private void Awake()
    {
        instance = this;
        //
        CreateLayerHash();
        SetDirs();
    }
    //
    private void Start()
    {
        EffectManager.instance.CreateEffect();
    }
    //====================================//
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
            elecShooter.isElevate = true;
        //if (Input.GetKeyDown(KeyCode.F2))
        //    SoundManager.instance.PlayBGM(SoundKey.BGM);
    }
    //====================================//
    public RaycastHit2D RayToDirs(Transform tf, int dir, int layer)
    {
        float dist = dirs[dir].magnitude;
        RaycastHit2D hit = Physics2D.Raycast(tf.position, dirs[dir], dist, layer);
        return hit;
    }
    //====================================//
    private void SetDirs()
    {
        dirs.Add(new Vector3(+64.0f, 0, 0));
        dirs.Add(new Vector3(+32.0f, +47.0f, 0));
        dirs.Add(new Vector3(-32.0f, +47.0f, 0));
        dirs.Add(new Vector3(-64.0f, 0, 0));
        dirs.Add(new Vector3(-32.0f, -47.0f, 0));
        dirs.Add(new Vector3(+32.0f, -47.0f, 0));
    }
    //
    private void CreateLayerHash()
    {
        footBoardLayer = LayerMask.GetMask("FootBoard");
        concreteLayer = LayerMask.GetMask("Concrete");
        closetLayer = LayerMask.GetMask("Closet");
        innerLayer = LayerMask.GetMask("Inner");
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