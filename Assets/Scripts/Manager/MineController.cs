﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineController : MonoBehaviour
{
    //=========================================//
    public static MineController instance;
    //=========================================//
    public float crashTime = 1.0f;
    public float crashWaitTime = 0.5f;
    WaitForSeconds cwt;
    public float crashReturnTime = 3.0f;
    //=========================================//
    public GameObject ghostPrefab;
    [SerializeField] private List<GameObject> ghosts;
    //=========================================//
    private void Awake()
    {
        instance = this;
        cwt = new WaitForSeconds(crashWaitTime);
        //
        CreateGhosts();
    }
    //=========================================//
    private void ActivateGhost(Transform tf)
    {
        foreach (GameObject g in ghosts)
        {
            if (g.activeSelf == true)
                continue;
            //
            Vector3 pos = tf.position;
            pos.z = g.transform.position.z;
            g.transform.position = pos;
            g.SetActive(true);
            return;
        }
    }
    //
    private void CreateGhosts()
    {
        ghostPrefab = Resources.Load<GameObject>("Prefabs/Ghost");
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
            g.SetActive(false);
            //
            ghosts.Add(g);
        }
    }
    //=========================================//
       private void SetCrashMine(Transform tf)
    {
        //Debug.Log("Crash At " + tf.position);
        Transform floorTF = tf.parent.parent.parent;
        Transform stageTF = floorTF.parent;
        //
        int floorNum = floorTF.name[0] - '0';
        int stageNum = stageTF.name[0] - '0';
        //
        Floor floor = Map.instance.GetFloor(stageNum, floorNum);
        //
        Transform concA = floor.concreteTiles[2].transform;
        bool isRight = (concA.position.x - tf.position.x) > 0;
        //
        Transform concB = isRight ?
            floor.concreteTiles[0].transform : floor.concreteTiles[1].transform;
        //
        Vector3 targetPos = tf.position;
        targetPos.z = -1;
        Vector3 targetPosA = targetPos;
        Vector3 targetPosB = targetPos;
        float tileHalfSize = 32;
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
        StartCoroutine(GoToTarget(concA, concB, targetPosA, targetPosB));
    }
    //
    private IEnumerator BackToOrigin(Transform concA, Transform concB,
        Vector3 targetPosA, Vector3 targetPosB)
    {
        float t = 0;
        float speed = 1.0f / crashReturnTime;
        //
        Vector3 startPosA = concA.position;
        Vector3 startPosB = concB.position;
        while (true)
        {
            if (t >= crashReturnTime)
            {
                concA.position = targetPosA;
                concB.position = targetPosB;
                yield break;
            }
            else
            {
                t += Time.deltaTime;
                //
                concA.position = Vector3.Lerp(startPosA, targetPosA, t*speed);
                concB.position = Vector3.Lerp(startPosB, targetPosB, t*speed);
            }
            yield return null;
        }
    }
    //
    private IEnumerator GoToTarget(Transform concA, Transform concB,
        Vector3 targetPosA, Vector3 targetPosB)
    {
        Vector3 originPosA = concA.position;
        Vector3 originPosB = concB.position;
        //
        float distA = 0, distB = 0;
        //
        // 등가속도 직선운동
        // s = v0*t + 1/2 * a * t^2;
        // a = 2*s / t^2;
        float t = crashTime;
        //
        float sA = targetPosA.x - originPosA.x;
        float aA = (2.0f * sA) / Mathf.Pow(t, 2.0f);
        float sB = targetPosB.x - originPosB.x;
        float aB = (2.0f * sB) / Mathf.Pow(t, 2.0f);
        //
        float vA = 0, vB = 0;
        //
        t = 0;
        //
        while (true)
        {
            if (t >= crashTime)
            {
                concA.position = targetPosA;
                concB.position = targetPosB;
                yield return cwt;
                StartCoroutine(BackToOrigin(concA, concB, originPosA, originPosB));
                yield break;
            }
            else
            {
                t += Time.deltaTime;
                //aA += 1.0f * Time.deltaTime;
                //aB += 1.0f * Time.deltaTime;
                ////
                vA += aA * Time.deltaTime;
                vB += aB * Time.deltaTime;
                //
                Vector3 tempA = concA.position;
                tempA.x += (vA * Time.deltaTime);
                concA.position = tempA;
                //
                Vector3 tempB = concB.position;
                tempB.x += (vB * Time.deltaTime);
                concB.position = tempB;
            }

            yield return null;
        }
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
    public void ActivateMine(MineTypes type, Transform tf = null)
    {
        switch (type)
        {
            case MineTypes.NONE:
                break;
            case MineTypes.PULL:
            case MineTypes.PUSH:
                SetExplosion(type, tf);
                break;
            case MineTypes.NARROWING:
                GameManager.instance.elecShooter.UpLevel();
                break;
            case MineTypes.CRASH:
                SetCrashMine(tf);
                break;
            case MineTypes.GHOST:
                ActivateGhost(tf);
                break;
            case MineTypes.LIGHTNING:
                break;
            default:
                break;
        }
    }
    //=========================================//
}
