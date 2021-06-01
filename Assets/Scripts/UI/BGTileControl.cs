using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BGTileControl : MonoBehaviour
{
    public float scrollingSpeed;
    //
    private int hashFlip;
    private const float floorHeight = 188.0f;
    private const float scrollEndPosY = 700.0f;
    private float timeToNextFlip;
    //
    public List<Transform> floors = new List<Transform>();
    private List<Transform> tileInScreen = new List<Transform>();
    //=================================================//
    private void Awake()
    {
        // Set Floors
        for(int i = 0; i < transform.childCount; ++i)
            floors.Add(transform.GetChild(i).GetComponent<Transform>());
        //
        hashFlip = Animator.StringToHash("BGTileFlip");
    }
    //
    private void Update()
    {
        Scrolling();
        RandomFlip();
    }
    //
    private void OnDisable()
    {
        DOTween.KillAll();
    }
    //=================================================//
    private void Scrolling()
    {
        for(int i = 0; i < floors.Count; ++i)
        {
            floors[i].localPosition += Vector3.up * scrollingSpeed * Time.deltaTime;
            if(floors[i].localPosition.y > scrollEndPosY)
            {
                Transform t = floors[i];
                t.localPosition += Vector3.down * floors.Count * floorHeight;
                floors.RemoveAt(i);
                floors.Add(t);
            }
        }
    }
    //
    private void RandomFlip()
    {
        // check time to flip
        timeToNextFlip -= Time.deltaTime;
        if (timeToNextFlip > 0)
            return;
        // set time to next flip(1,2,3,4,5)
        timeToNextFlip = Random.Range(1, 6);
        // set count to flip (1,2,3)
        int count = Random.Range(1, 4);
        // set tileInScreen
        tileInScreen.Clear();
        for (int i = 1; i < floors.Count - 1; ++i) // except first & last floor
            for (int j = 0; j < floors[0].childCount; ++j)
                tileInScreen.Add(floors[i].GetChild(j));
        //
        MyUtility.ShuffleList(tileInScreen);
        //
        Sequence sq = DOTween.Sequence();

        for(int i = 0; i < count; ++i)
        {
            Animator anim = tileInScreen[i].GetComponent<Animator>();
            TweenCallback flip = () => { anim.SetTrigger(hashFlip); };
            sq.AppendCallback(flip);
            sq.AppendInterval(0.1f);
        }
    }
}