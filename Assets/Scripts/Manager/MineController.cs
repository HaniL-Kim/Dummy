using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==================// //==================//
[System.Serializable]
public struct DisplayTexture_Num
{
    public NumberIcons number;
    public Sprite texture;
}
//
[System.Serializable]
public struct DisplayTexture_Mine
{
    public MineTypes mineType;
    public Sprite texture;
}
//
public class MineController : MonoBehaviour
{
    //=========================================//
    public static MineController instance;
    //=========================================//
    public DisplayTexture_Mine[] mineIcons;
    public DisplayTexture_Num[] numIcons;
    //====================================//
    public enum Difficulty { NORMAL, HARD, IMPOSSIBLE }
    public Difficulty difficulty = Difficulty.NORMAL;
    //=========================================//
    public float crashTime = 1.0f;
    public float crashWaitTime = 0.5f;
    public float crashReturnTime = 3.0f;
    //=========================================//
    public GameObject ghostPrefab;
    public List<GameObject> ghosts;
    //=========================================//
    private void Awake()
    {
        instance = this;
        //
        CreateGhosts();
    }
    //=========================================//
    private void Update()
    {
        DebugGhost();
    }
    //=========================================//
    public void SetDifficulty(string diff)
    {
        if (diff == "NORMAL")
            difficulty = Difficulty.NORMAL;
        else if (diff == "HARD")
            difficulty = Difficulty.HARD;
        else if (diff == "IMPOSSIBLE")
            difficulty = Difficulty.IMPOSSIBLE;
    }
    //
    public void DebugGhost()
    {
        if (Input.GetKeyDown(KeyCode.I))
        { // 1체 생성
            ActivateGhost(Camera.main.transform);
        }
        else if (Input.GetKeyDown(KeyCode.O))
        { // 1체 파괴
            foreach (GameObject g in ghosts)
                if (g.activeSelf == true)
                {
                    g.GetComponent<Ghost>().Dead();
                    return;
                }
        }
        else if (Input.GetKeyDown(KeyCode.P))
        { // 전체 파괴
            foreach (GameObject g in ghosts)
            {
                if (g.activeSelf == true)
                    g.GetComponent<Ghost>().Dead();
                else
                    return;
            }
        }
    }
    //=========================================//
    private void ActivateGhost(Transform tf = null)
    {
        foreach (GameObject g in ghosts)
        {
            if (g.activeSelf == true)
                continue;
            //
            Vector3 pos = Vector3.zero;
            if (tf != null)
                pos = tf.position;
            //
            pos.z = g.transform.position.z;
            g.transform.position = pos;
            g.SetActive(true);
            return;
        }
    }
    //
    private void CreateGhosts()
    {
        ghostPrefab = Resources.Load<GameObject>("Prefabs/Character/Ghost");
        //
        GameObject ghostHolder = new GameObject("GhostHolder");
        ghostHolder.transform.SetParent(transform);
        //
        ghosts = new List<GameObject>();
        //
        int ghostCount = 20;
        for (int i = 0; i < ghostCount; ++i)
        {
            GameObject g = Instantiate<GameObject>(ghostPrefab);
            g.transform.SetParent(ghostHolder.transform);
            g.transform.name = "Ghost_" + i.ToString();
            g.SetActive(false);
            //
            ghosts.Add(g);
        }
    }
    //=========================================//
       private void SetCrashMine(Inner inner)
    {
        Floor floor = inner.GetFloor();
        Transform tileTF = inner.transform.parent;

        //
        Transform concA = floor.concreteTiles[2].transform;
        bool isRight = (concA.localPosition.x - tileTF.localPosition.x) > 0;
        //
        Transform concB = isRight ?
            floor.concreteTiles[0].transform : floor.concreteTiles[1].transform;
        //
        Vector3 targetPos = tileTF.localPosition;
        Vector3 targetPosA = targetPos;
        Vector3 targetPosB = targetPos;
        //
        float tileHalfSize = Map.TileWidth * 0.5f;
        //
        if (isRight)
        {
            targetPosA.x += tileHalfSize;
            targetPosB.x -= tileHalfSize;
        }
        else
        {
            targetPosA.x -= tileHalfSize;
            targetPosB.x += tileHalfSize;
        }
        //
        concA.GetComponent<Concrete>().StartToMove(targetPosA);
        concB.GetComponent<Concrete>().StartToMove(targetPosB);
    }
    //=========================================//
    private void SetExplosion(MineTypes type, Transform tf)
    {
        switch (type)
        {
            case MineTypes.PULL:
                EffectManager.instance.Play("Explosion", tf);
                break;
            case MineTypes.PUSH:
                {
                    for (int i = 0; i < 6; ++i)
                    {
                        // Check 6 dir tiles that is Concrete
                        RaycastHit2D hit = GameManager.instance.RayToDirs(
                            tf, i, GameManager.instance.concreteLayer);
                        if (hit)
                        {
                            if (hit.collider.CompareTag("Concrete"))
                                continue;
                        }
                        else
                        {
                            Vector3 pos = tf.position + GameManager.instance.dirs[i];
                            pos.z = -5.0f;
                            EffectManager.instance.Play("Explosion", pos, Quaternion.identity);
                        }
                    }
                }
                break;
            default:
                break;
        }
    }
    //=========================================//
    public void BeginAlert(MineTypes type, Inner inner)
    {
        switch (type)
        {
            case MineTypes.NONE: break;
            case MineTypes.PULL:
                {
                    if (difficulty == Difficulty.HARD)// || difficulty == Difficulty.IMPOSSIBLE)
                        inner.FlipSides();
                    //
                    EffectManager.instance.Play("Pull", inner.transform);
                }
                break;
            case MineTypes.PUSH:
                {
                    if (difficulty == Difficulty.HARD)// || difficulty == Difficulty.IMPOSSIBLE)
                        inner.FlipArround(false);
                    //
                    EffectManager.instance.Play("Push", inner.transform);
                }
                break;
            case MineTypes.GHOST: break;
            case MineTypes.THUNDER: break;
            case MineTypes.NARROWING: break;
            case MineTypes.CRASH:
                {
                    if (difficulty == Difficulty.HARD)// || difficulty == Difficulty.IMPOSSIBLE)
                    {
                        Floor floor = inner.GetComponent<Inner>().GetFloor();
                        foreach (GameObject item in floor.tiles)
                            item.GetComponent<Tile>().closet.Flip();
                    }
                }
                break;
            default:
                break;
        }
    }
    //
    public void ActivateMine(Inner inner)
    {
        switch (inner.mineType)
        {
            case MineTypes.NONE: break;
            case MineTypes.PULL:
            case MineTypes.PUSH:
                SetExplosion(inner.mineType, inner.transform);
                break;
            case MineTypes.GHOST:
                ActivateGhost(inner.transform);
                break;
            case MineTypes.THUNDER:
                EffectManager.instance.Play("Thunder", inner.transform);
                break;
            case MineTypes.NARROWING:
                GameManager.instance.elecShooter.UpLevel();
                break;
            case MineTypes.CRASH:
                SetCrashMine(inner);
                break;
            default:
                break;
        }
    }
    //=========================================//
}
