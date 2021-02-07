using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElecShooterController : MonoBehaviour
{
    //==========================================//
    public enum NarrowLevel
    { NONE, ONE, TWO, THREE }
    //==========================================//
    public GameObject Up;
    public GameObject Down;
    //
    public List<Sprite> narrowLevelSprites;
    public List<Sprite> narrowLevelTimeSprites;
    public List<SpriteRenderer> NarrowLevels; // UL, UR, DL, DR
    public List<SpriteRenderer> NarrowLevelTimes; // UL, UR, DL, DR
    //==========================================//
    private Transform tf;
    //==========================================//
    public bool isElevate = false;
    public float elevateSpeed;
    public NarrowLevel level;
    //
    public float shooterMovingSpeed = 1.0f;
    //
    public readonly float _maxCountTime = 8.0f;
    public float countDownTimer = 0.0f;
    //==========================================//
    private void Awake()
    {
        tf = GetComponent<Transform>();
    }
    private void Start()
    {
        foreach (SpriteRenderer sr in NarrowLevels)
        {
            NarrowLevelTimes.Add(sr.transform.GetChild(0).GetComponent<SpriteRenderer>());
        }
    }
    //==========================================//
    private void Update()
    {
        Elevate();
        CountDown();
        Test();
    }
    //==========================================//
    private void Test()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
            SetLevelCoroutine(0);
        else if (Input.GetKeyDown(KeyCode.Alpha1))
            SetLevelCoroutine(1);
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            SetLevelCoroutine(2);
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            SetLevelCoroutine(3);
        else if (Input.GetKeyDown(KeyCode.Alpha4))
            UpLevel();
        else if (Input.GetKeyDown(KeyCode.Alpha5))
            DownLevel();
    }
    //==========================================//
    public void UpLevel()
    { // Call When Mine Activate
        level += 1;
        if ((int)level > 3)
            level = NarrowLevel.THREE;
        SetLevelCoroutine((int)level);
    }
    //
    public void DownLevel()
    { // Call When CountDown End
        level -= 1;
        if ((int)level < 1)
            level = 0;
        SetLevelCoroutine((int)level);
    }
    //
    private void CountDown()
    {
        if (level == NarrowLevel.NONE)
            return;
        else
        {
            countDownTimer += Time.deltaTime;
            if (countDownTimer >= _maxCountTime)
            {
                countDownTimer = 0;
                DownLevel();
            }
            //
            SetLevelTimeSprite();
        }
    }
    //
    private void Elevate()
    {
        if (isElevate == false)
            return;
        //
        tf.Translate(Vector3.up * elevateSpeed * Time.deltaTime);
    }
    //
    private void SetLevelSprite(int value)
    {
        foreach (SpriteRenderer item in NarrowLevels)
        {
            if (value == 0)
                item.sprite = null;
            else
                item.sprite = narrowLevelSprites[value - 1];
        }
    }
    //
    private void SetLevelTimeSprite()
    {
        foreach (SpriteRenderer sr in NarrowLevelTimes)
        {
            if (countDownTimer == 0)
                sr.sprite = null;
            else
                sr.sprite = narrowLevelTimeSprites[(int)countDownTimer];
        }
    }
    private IEnumerator SetLevel(int value)
    {
        countDownTimer = 0;
        SetLevelTimeSprite();
        SetLevelSprite(value);
        //
        level = (NarrowLevel)value;
        //
        Vector3 up_lp, up_target, up_dir, up_dirN, up_pos;
        Vector3 down_lp, down_target, down_dir, down_dirN, down_pos;
        //
        float up_target_y = 470.0f - (float)level * 47.0f;
        float down_target_y = 000.0f + (float)level * 47.0f;
        //
        {
            up_lp = Up.transform.localPosition;
            up_target = up_lp;
            up_target.y = up_target_y;
            up_dirN = (up_target - up_lp).normalized;
        }
        {
            down_lp = Down.transform.localPosition;
            down_target = down_lp;
            down_target.y = down_target_y;
            down_dirN = (down_target - down_lp).normalized;
        }
        //
        float debugTime = 0;
        while (true)
        {
            debugTime += Time.deltaTime;
            //
            {
                up_lp = Up.transform.localPosition;
                up_dir = (up_target - up_lp);
                //
                down_lp = Down.transform.localPosition;
                down_dir = (down_target - down_lp);
            }
            //
            if (up_dir.magnitude < 1.0f || down_dir.magnitude < 1.0f)
            {
                Up.transform.localPosition = up_target;
                Down.transform.localPosition = down_target;
                //print("Shooter Move Time : " + debugTime);
                yield break;
            }
            //
            up_pos = up_lp + up_dirN * shooterMovingSpeed * Time.deltaTime;
            Up.transform.localPosition = up_pos;
            //
            down_pos = down_lp + down_dirN * shooterMovingSpeed * Time.deltaTime;
            Down.transform.localPosition = down_pos;
            //
            yield return null;
        }
    }
    //
    private void SetLevelCoroutine(int value)
    {
        StopAllCoroutines();
        StartCoroutine(SetLevel(value));
    }
    //
    //==========================================//
}
