using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Ghost : MonoBehaviour
{
    public float maxForce = 350.0f;
    //=============================================//
    List<string> deadTags = new List<string>()
        { "Explosion", "Thunder", "Player" };
    // public Sprite deadSprite;
    //=============================================//
    public enum GhostState { MOVE, STOP, DEAD }
    public List<AudioClip> audioClips;
    //=============================================//
    // anim hash
    private readonly int hashStopTrigger =
        Animator.StringToHash("Ghost_Stop");
    private readonly int hashDeadTrigger =
        Animator.StringToHash("Ghost_Dead");
    // shader hash
    private readonly int hashOutlineTK =
        Shader.PropertyToID("_OutlineThickness");
    //=============================================//
    public GhostState state = GhostState.MOVE;
    //=============================================//
    public int ghostExplodeDelayLevel = 1;
    //
    public float ghostMoveSpeed = 10.0f;
    public float moveTime;
    [SerializeField] private float moveTimeCounter = 0;
    [SerializeField] private float randValue = 0;
    //
    [ColorUsageAttribute(true, true)]
    public Color outlineColorBegin;
    [ColorUsageAttribute(true, true)]
    public Color outlineColorEnd;
    //=============================================//
    private Vector3 dirToPlayer = Vector3.zero;
    //=============================================//
    private Transform playerTF;
    private ElecShooterController elecShooter;
    //=============================================//
    private Transform tf;
    private SpriteRenderer sr;
    private Animator anim;
    private Rigidbody2D rb;
    private Collider2D col;
    public AudioSource audioSRC;
    //=============================================//
    private void Awake()
    {
        tf = GetComponent<Transform>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        audioSRC = GetComponent<AudioSource>();
        //
        //moveTime = 10.0f;
        moveTimeCounter = moveTime;
        //
        sr.material.SetColor("_OutlineColor_Inner", outlineColorBegin);
    }
    //
    private void Start()
    {
        playerTF = GameManager.instance.dummy.transform;
        elecShooter = GameManager.instance.elecShooter
            .GetComponent<ElecShooterController>();
        //
        Outline(true);
    }
    //
    private void OnEnable()
    {
        randValue = Random.Range(0.7f, 1.3f);
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
        sr.flipX = (rb.velocity.x >= 0); // 이동 기준
        //rend.flipX = (dirToPlayer.x >= 0); // 힘 기준
    }
    //
    private void Move()
    {
        Vector3 temp = tf.position;
        // Sync Ghost PosY with ElecShooter Move
        float speed = 0;
        if (elecShooter.isElevate == true)
            speed = elecShooter.elevateSpeed;
        //
        temp.y += speed * Time.deltaTime;
        tf.position = temp;
        //
        switch (state)
        {
            case GhostState.MOVE:
                {
                    Vector2 force =
                        dirToPlayer * randValue * Time.deltaTime * ghostMoveSpeed;
                    //
                    if (force.magnitude > maxForce)
                    {
                        float ratio = maxForce / force.magnitude;
                        force *= ratio;
                    }
                    //
                    rb.AddForce(force);
                }
                break;
            case GhostState.STOP:
            case GhostState.DEAD:
                break;
            default:
                break;
        }
        //
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
            sr.material.SetFloat(hashOutlineTK, 4.0f);
            //EffectManager.instance.SetBloom(value);
        }
        else
        {
            sr.material.SetFloat(hashOutlineTK, 0);
            //EffectManager.instance.SetBloom(value);
        }
    }
    private void EndMove()
    {
        state = GhostState.STOP;
        anim.SetTrigger(hashStopTrigger);
        //col.enabled = false;
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
        //
        audioSRC.PlayOneShot(audioClips[0]);
    }
    //
    private void SetToDefault()
    {
        DOTween.Kill(transform.name + "_TweenMatColor");
        //
        state = GhostState.MOVE;
        //
        anim.enabled = true;
        col.enabled = true;
        //
        moveTimeCounter = moveTime;
        //
        sr.material.SetColor("_OutlineColor_Inner", outlineColorBegin);
        Outline(true);
        //
        gameObject.SetActive(false);
    }
    //
    private void Explode()
    {
        EffectManager.instance.Play("Explosion", tf.position, tf.rotation);
        //
        SetToDefault();
    }
    //
    private void OutlineColorChange(float time)
    {
        sr.material.DOColor(outlineColorEnd, "_OutlineColor_Inner", time)
            .SetId(transform.name + "_TweenMatColor");
    }
    //
    public void ExplodeAfterDelay()
    {
        float ghostExplodeDealy = 1.0f * (float)ghostExplodeDelayLevel;
        OutlineColorChange(ghostExplodeDealy);
        Invoke("Explode", ghostExplodeDealy);
    }
    //
    public void Dead()
    {
        state = GhostState.DEAD;
        //
        //anim.enabled = false;
        col.enabled = false;
        //
        //sr.sprite = deadSprite;
        anim.SetTrigger(hashDeadTrigger);
        //
        audioSRC.PlayOneShot(audioClips[1]);
    }
    //=============================================//
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (state == GhostState.DEAD)
            return;
        //
        foreach (string tag in deadTags)
            if (collision.CompareTag(tag) == true)
            {
                Dead();
                //if(tag == "Player")
                //{
                //Dummy d = collision.GetComponent<Dummy>();
                //switch (d.shieldState)
                //{
                //    case ShieldEffect.ShieldState.NONE:
                //        Dead();
                //        break;
                //    case ShieldEffect.ShieldState.NORMAL:
                //        {
                //            //d.ActiveShield();
                //            Dead();
                //        }
                //        break;
                //    case ShieldEffect.ShieldState.ACTIVE:
                //        break;
                //    default:
                //        break;
                //}
                //}
                //else
            }
    }
    //=============================================//
}
