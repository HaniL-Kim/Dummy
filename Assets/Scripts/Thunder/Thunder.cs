using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Thunder : MonoBehaviour
{
    //====================================//
    EffectType type = EffectType.THUNDER;
    //
    public float vSpeed = 70.0f, hSpeed = 30.0f;
    public float thunderWaitingTime = 1.0f;
    private WaitForSeconds ws;
    //====================================//
    // for Debug(Check flag state)
    [SerializeField] private bool isExplode = false;
    //====================================//
    // body
    public ThunderBody tb;
    public ThunderHead th_U;
    public ThunderHead th_D;
    //====================================//
    // Shader Color
    [ColorUsageAttribute(true, true)]
    public Color outlineColorBegin;
    [ColorUsageAttribute(true, true)]
    public Color outlineColorEnd;
    [ColorUsageAttribute(true, true)]
    public Color outlineColorExplosion;
    // 
    public Color disColor;
    // Shader Hash
    private readonly int hashOutlineTK =
        Shader.PropertyToID("_OutlineThickness");
    private readonly int hashOutlineColor =
        Shader.PropertyToID("_OutlineColor_Inner");
    //
    public float outline_Thickness;
    public float disappearTime;
    //====================================//
    // ElecShooter
    public ElecShooterController ec;
    private Transform e_Up, e_Down;
    //====================================//
    // Player
    private Transform player;
    //====================================//
    // Component
    private Transform tf;
    //====================================//
    Vector3 temp = Vector3.zero;
    //====================================//
    private void Awake()
    {
        tf = GetComponent<Transform>();
        //
        tb = tf.GetComponentInChildren<ThunderBody>();
        th_U = tf.GetChild(1).GetComponent<ThunderHead>();
        th_D = tf.GetChild(2).GetComponent<ThunderHead>();
        //
        ws = new WaitForSeconds(thunderWaitingTime);
        //
        SetShader();
    }
    //
    private void Start()
    {
        player = GameManager.instance.dummy.transform;
        ec = GameManager.instance.elecShooter;
        //
        //ec = GameObject.FindGameObjectWithTag("ElecShooter")
        //    .GetComponent<ElecShooterController>();
        e_Up = ec.Up.transform;
        e_Down = ec.Down.transform;
    }
    //====================================//
    private void Update()
    {
        MoveToPlayer();
        FollowHeads();
        AttachHeads();
        tb.SetBody();
        CheckReady();
    }
    //====================================//
    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    if (head_U)
    //        Gizmos.DrawSphere(head_U.position, 0.5f);
    //    //
    //    Gizmos.color = Color.blue;
    //    if (head_D)
    //        Gizmos.DrawSphere(head_D.position, 0.5f);
    //}
    //====================================//
    public void SetShader()
    {
        tb.SetMat(hashOutlineColor, outlineColorBegin, hashOutlineTK, outline_Thickness);
        th_U.SetMat(hashOutlineColor, outlineColorBegin, hashOutlineTK, outline_Thickness);
        th_D.SetMat(hashOutlineColor, outlineColorBegin, hashOutlineTK, outline_Thickness);
    }
    //
    public void ResetThunder()
    { // Call By ThunderBody when anim(ThunderExplode) Frame Event
        tb.ResetThunderBody(hashOutlineColor, outlineColorBegin);
        th_U.ResetThunderHead(hashOutlineColor, outlineColorBegin);
        th_D.ResetThunderHead(hashOutlineColor, outlineColorBegin);
        isExplode = false;
        //
        transform.SetParent(EffectManager.instance.effHolders[(int)type]);
        //
        gameObject.SetActive(false);
    }
    //
    private IEnumerator ThunderExplode()
    {
        isExplode = true;
        // Flip Tiles In Hard Mode
        switch (SceneControl.instance.currentDifficulty)
        {
            case Difficulty.NORMAL:
                break;
            case Difficulty.HARD:
                {
                    Collider2D[] closets = Physics2D.OverlapBoxAll(
                        tb.transform.position, tb.col.size, 0, GameManager.instance.closetLayer);
                    if (closets.Length != 0)
                    {
                        foreach (var item in closets)
                        {
                            item.GetComponent<Closet>().Flip();
                        }
                    }
                }
                break;
            //case Difficulty.IMPOSSIBLE:
            //    break;
            default:
                break;
        }
        //
        th_D.ColorTween(hashOutlineColor, outlineColorEnd, thunderWaitingTime);
        th_U.ColorTween(hashOutlineColor, outlineColorEnd, thunderWaitingTime);
        tb.ColorTween(hashOutlineColor, outlineColorEnd, thunderWaitingTime);
        //
        yield return ws;
        // isExplode = true;
        //
        tb.Explode(hashOutlineColor, outlineColorExplosion, disColor, disappearTime);
        //
        th_U.Explode(disColor, disappearTime);
        th_D.Explode(disColor, disappearTime);
        //
        SoundManager.instance.Play(SoundKey.THUNDER_EXPLOSION);
    }
    //
    private void CheckReady()
    {
        if (th_U.ready && th_D.ready)
            if(isExplode == false)
                StartCoroutine(ThunderExplode());
    }
    //
    private void MoveToPlayer()
    {
        if (player.GetComponent<Dummy>().isDead == true)
            return;
        if (th_U.ready && th_D.ready)
            return;
        //
        temp = Vector3.zero;
        temp.x = player.position.x - tf.position.x;
        if (Mathf.Abs(temp.x) > 1.0f)
            temp.Normalize();
        else
            return;
        //
        tf.Translate(temp * hSpeed * Time.deltaTime);
    }
    //
    private void AttachHeads()
    {
        if (th_U.ready)
        {
            th_U.SetPosY(e_Up.position.y);
        }
        if (th_D.ready)
        {
            th_D.SetPosY(e_Down.position.y);
        }
    }
    //
    private void FollowHeads()
    {
        float speed = GameManager.instance.elecShooter.elevateSpeed + vSpeed;
        th_U.FollowHead("<=", e_Up.position.y, Vector3.up, speed);
        th_D.FollowHead(">=", e_Down.position.y, Vector3.down, speed);
    }
    //====================================//
}