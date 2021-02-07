using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    public enum GhostState { MOVE, STOP }
    //=============================================//
    // anim hash
    private readonly int hashStopTrigger =
        Animator.StringToHash("Ghost_Stop");
    // shader hash
    private readonly int hashOutlineTK =
        Shader.PropertyToID("_OutlineThickness");
    //=============================================//
    public GhostState state = GhostState.MOVE;
    //
    public float moveTime;
    [SerializeField] private float moveTimeCounter = 0;
    [SerializeField] private float randValue = 0;
    //
    [ColorUsageAttribute(true, true)]
    public Color outlineColor;
    //
    private Vector3 dirToPlayer = Vector3.zero;
    //=============================================//
    private Transform playerTF;
    private ElecShooterController elecShooter;
    //=============================================//
    private Transform tf;
    private SpriteRenderer rend;
    private Animator anim;
    private Rigidbody2D rb;
    private Collider2D col;
    //=============================================//
    private void Awake()
    {
        tf = GetComponent<Transform>();
        rend = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        //
        moveTime = 10.0f;
        moveTimeCounter = moveTime;
        //
        rend.material.SetColor("_OutlineColor_Inner", outlineColor);
        //
        randValue = Random.Range(0.7f, 1.3f);
    }
    private void Start()
    {
        playerTF = GameManager.instance.player;
        elecShooter = GameManager.instance.elecShooter
            .GetComponent<ElecShooterController>();
        //
        Outline(true);
    }
    //=============================================//
    private void Update()
    {
        MoveTimer();
        SetDir();
        Move();
    }
    //=============================================//
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if(tf)
            Debug.DrawRay(tf.position, dirToPlayer);
    }
    //=============================================//
    private void SetDir()
    {
        if(MyUtility.IsNull(playerTF, name))
            return;
        //
        dirToPlayer = (playerTF.position - tf.position);
        //
        rend.flipX = (rb.velocity.x >= 0); // 이동 기준
        //rend.flipX = (dirToPlayer.x >= 0); // 힘 기준
    }
    //
    private void Move()
    {
        Vector3 temp = tf.position;
        //
        float speed = 0;
        if (elecShooter.isElevate == true)
            speed = elecShooter.elevateSpeed;
        //
        temp.y += speed * Time.deltaTime;
        //
        switch (state)
        {
            case GhostState.MOVE:
                rb.AddForce(dirToPlayer * randValue);
                break;
            case GhostState.STOP:
                break;
            default:
                break;
        }
        //
        tf.position = temp;
    }
    private void MoveTimer()
    {
        if (moveTimeCounter <= 0)
            return;
        //
        moveTimeCounter -= Time.deltaTime;
        if (moveTimeCounter <= 0)
            EndMove();
    }
    //
    public void Outline(bool value)
    {
        if(value == true)
        {
            rend.material.SetFloat(hashOutlineTK, 4.0f);
            //EffectManager.instance.SetBloom(value);
        }
        else
        {
            rend.material.SetFloat(hashOutlineTK, 0);
            //EffectManager.instance.SetBloom(value);
        }
    }
    private void EndMove()
    {
        state = GhostState.STOP;
        anim.SetTrigger(hashStopTrigger);
        col.enabled = false;
        Outline(false);
    }
    //=============================================//
    public void EndStop()
    { // Anim Frame Event(Ghost_Stop.clip : 300 Frame)
        state = GhostState.MOVE;
        moveTimeCounter = moveTime;
        //
        randValue = Random.Range(0.8f, 1.2f);
        //
        col.enabled = true;
        Outline(true);
    }
    //=============================================//
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("PlayerDead By Ghost");
            //collision.gameObject.SetActive(false);
        }
        if (collision.CompareTag("Concrete"))
        {
            //PushOutside();
        }
    }
    //=============================================//
}
