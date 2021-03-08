using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thunder : MonoBehaviour
{
    //====================================//
    EffectType type = EffectType.THUNDER;
    //
    public float vSpeed = 70.0f, hSpeed = 30.0f;
    public int thunderWaitingFrame = 10;
    private WaitForSeconds ws;
    //====================================//
    // for Debug(Check flag state)
    [SerializeField] private bool
        ready_U = false, ready_D = false, isExplode = false;
    //====================================//
    // body
    private ThunderBody tb;
    // head(Up, Down)
    [HideInInspector]
    public Transform head_U, head_D;
    private Animator anim_U, anim_D;
    //====================================//
    private readonly int hashBodyExplode =
        Animator.StringToHash("ThunderExplode");
    private readonly int hashHeadReady =
        Animator.StringToHash("ThunderReady");
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
        //
        head_U = tf.GetChild(1).transform;
        anim_U = head_U.GetComponent<Animator>();
        //
        head_D = tf.GetChild(2).transform;
        anim_D = head_D.GetComponent<Animator>();
        //
        ws = new WaitForSeconds((float)thunderWaitingFrame / 60.0f);
    }
    //
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        //
        ec = GameObject.FindGameObjectWithTag("ElecShooter")
            .GetComponent<ElecShooterController>();
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
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (head_U)
            Gizmos.DrawSphere(head_U.position, 0.5f);
        //
        Gizmos.color = Color.blue;
        if (head_D)
            Gizmos.DrawSphere(head_D.position, 0.5f);
    }
    //====================================//
    public void ResetThunder()
    { // Call By ThunderBody when anim(ThunderExplode) Frame Event
        head_U.GetComponent<SpriteRenderer>().color = Color.white;
        head_D.GetComponent<SpriteRenderer>().color = Color.white;
        //
        head_U.localPosition = new Vector3(0, 0, -1);
        head_D.localPosition = new Vector3(0, 0, -1);
        //
        ready_U = false;
        ready_D = false;
        isExplode = false;
        //
        transform.SetParent(EffectManager.instance.effHolders[(int)type]);
        //
        gameObject.SetActive(false);
    }
    //
    private IEnumerator ThunderExplode()
    {
        // Flip Tiles In Hard Mode
        Collider2D[] closets = Physics2D.OverlapBoxAll(
            transform.position, tb.col.size, 0, GameManager.instance.closetLayer);
        if(closets.Length != 0)
        {
            foreach (var item in closets)
            {
                item.GetComponent<Closet>().Flip();
            }
        }
        //
        yield return ws;
        //
        isExplode = true;
        //
        tb.Explode();
        //
        anim_U.SetTrigger(hashBodyExplode);
        anim_D.SetTrigger(hashBodyExplode);
    }
    //
    private void CheckReady()
    {
        if (ready_U && ready_D)
            if(isExplode == false)
                StartCoroutine(ThunderExplode());
    }
    //
    private void MoveToPlayer()
    {
        if (ready_U && ready_D)
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
        if (ready_U)
        {
            temp = head_U.position;
            temp.y = e_Up.position.y;
            head_U.position = temp;
        }
        if (ready_D)
        {
            temp = head_D.position;
            temp.y = e_Down.position.y;
            head_D.position = temp;
        }
    }
    //
    private void FollowHeads()
    {
        float speed = GameManager.instance.elecShooter.elevateSpeed + vSpeed;
        if(ready_U == false)
        {
            if (head_U.transform.position.y <= e_Up.position.y)
                head_U.Translate(Vector3.up * speed * Time.deltaTime);
            else
            {
                ready_U = true;
                anim_U.SetTrigger(hashHeadReady);
            }
        }
        //
        if(ready_D == false)
        {
            if (head_D.transform.position.y >= e_Down.position.y)
                head_D.Translate(Vector3.down * speed * Time.deltaTime);
            else
            {
                ready_D = true;
                anim_D.SetTrigger(hashHeadReady);
            }
        }
    }
    //====================================//
}