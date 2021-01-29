using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : MonoBehaviour
{
    //===============================//
    // Physics //
    // move
    public float moveSpeed;
    Vector2 moveVelocity;
    // jump-Up
    public float jumpForce;
    // jump-Down
    public Collider2D[] curFootBoard;
    // dash
    public int dashCount = 0;
    public float dashSpeed;
    private float dashTimeCounter;
    public float dashTime;
    private Vector2 dashDir = Vector2.zero;
    // flag
    public float flagTime;
    //===============================//
    public enum PlayerMode { FLAG, DASH }
    public PlayerMode mode = PlayerMode.FLAG;
    // Flags //
    public bool isWalk;
    public bool isJump;
    public bool isCrouch;
    public bool isScan;
    public bool isDash;
    //
    public bool onFootBoard;
    public bool onConcrete;
    //===============================//
    // Idle //
    public bool isIdle;
    public float idleCounter;
    public float idleTime;
    //===============================//
    // Component //
    private Transform tf;
    private SpriteRenderer rend;
    private Animator anim;
    private Rigidbody2D rb;
    private BoxCollider2D col;
    //===============================//
    // Collider
    public enum ColState { NORMAL, CROUCH };
    private Vector2[] colOffset = new Vector2[2]; // 0:normal, 1:crouch
    private Vector2[] colSize = new Vector2[2]; // 0:normal, 1:crouch
    //===============================//
    // Eye
    private Eye eye;
    //===============================//
    // GroundCheck
    private Vector2 footPos = Vector2.zero;
    private float footCircleSize = 5.0f;
    //=========================================//
    // Scan : HoldTile
    private Transform tileHolding;
    //=========================================//
    // anim Hash
    private readonly int dashTriggerHash = Animator.StringToHash("dashTrigger");
    private readonly int isDashHash = Animator.StringToHash("isDash");
    private readonly int isJumpHash = Animator.StringToHash("isJump");
    private readonly int isWalkHash = Animator.StringToHash("isWalk");
    private readonly int isCrouchHash = Animator.StringToHash("isCrouch");
    private readonly int verticalVelocityHash = Animator.StringToHash("verticalVelocity");
    // shader Hash
    private readonly int hashOutlineTK = Shader.PropertyToID("_OutlineThickness"); // 
    //=========================================//
    private void Start()
    {
        tf = GetComponent<Transform>();
        rend = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        //
        colOffset[0] = new Vector2(0, +0.5f);
        colOffset[1] = new Vector2(0, -1.0f);
        colSize[0] = new Vector2(13.0f, 28.0f);
        colSize[1] = new Vector2(13.0f, 25.0f);
        //
        eye = tf.GetChild(0).GetComponent<Eye>();
        eye.dummy = this;
        //
        dashTimeCounter = dashTime;
        //
    } // End Start
    //=========================================//
    private void Update()
    {
        Control();
        UIManager.instance.dashCount = dashCount;
    } // End Update
    private void FixedUpdate()
    {
        Move();
        Idle();
        EyePos();
        //
        GroundCheck();
    } // End FixedUpdate
    //=========================================//
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(footPos, footCircleSize);
    }
    //=========================================//
    public void Outline(bool value)
    {
        if(value == true)
        {
            rend.material.SetFloat(hashOutlineTK, 4.0f);
            EffectManager.instance.SetBloom(value);
        }
        else
        {
            rend.material.SetFloat(hashOutlineTK, 0);
            EffectManager.instance.SetBloom(value);
        }
    }
    //=========================================//
    private void Control()
    {
        Walk();
        Jump();
        Crouch();
        // LBtn
        Scan();
        // RBtn
        switch (mode)
        {
            case PlayerMode.FLAG:
                Flag();
                break;
            case PlayerMode.DASH:
                Dash();
                break;
            default:
                break;
        }
    } // End Control()
    private void Walk()
    {
        moveVelocity.x = Input.GetAxisRaw("Horizontal");
    } // End Walk()
    private void Move()
    {
        if (isCrouch == true)
            return;
        //
        if (moveVelocity.x != 0)
        {
            anim.SetBool(isWalkHash, (isWalk = true));
            //
            moveVelocity.Normalize();
            //
            if (moveVelocity.x < 0)
            {
                rend.flipX = false;
                eye.FlipX(rend.flipX);
            }
            else if (moveVelocity.x > 0)
            {
                rend.flipX = true;
                eye.FlipX(rend.flipX);
            }
        }
        else
        {
            anim.SetBool(isWalkHash, (isWalk = false));
        }
        //
        tf.Translate(moveVelocity * moveSpeed * Time.deltaTime);
    } // End Move
    private void Jump()
    {
        if (isJump == true)
            return;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(isCrouch == true) // Down Jump
            {
                if (onFootBoard == true)
                {
                    SetFootBoardTrigger(true);
                }
            }
            else // Up Jump
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    } // End Jump
    private void Crouch()
    {
        if (Input.GetKey(KeyCode.S))
        {
            if ((isJump == true) || (isCrouch == true))
                return;

            // Set Anim
            anim.SetBool(isCrouchHash, (isCrouch = true));
            // Set Collider
            col.offset = colOffset[(int)ColState.CROUCH];
            col.size = colSize[(int)ColState.CROUCH];
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            // Set Anim
            anim.SetBool(isCrouchHash, (isCrouch = false));
            // Set Collider
            col.offset = colOffset[(int)ColState.NORMAL];
            col.size = colSize[(int)ColState.NORMAL];
        }
    } // End Crouch
    private void Scan()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Transform tf1 = GameManager.instance.GetTileTransformOnMouse();
            if (tf1 != null)
            {
                tileHolding = tf1;
                tileHolding.localPosition = new Vector3(-1, +1, 0);
            }
            else
                tileHolding = null;
        }
        //
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            if (tileHolding != null)
                tileHolding.localPosition = Vector3.zero;
            //
            Transform tf2 = GameManager.instance.GetTileTransformOnMouse();
            if (tf2 == null)
                return;
            //
            if (tf2 == tileHolding)
            {
                isScan = true;
                eye.Scan();
                Outline(true);
                //
                tf2.GetComponent<Closet>().Flip();
                //
                EffectManager.instance.Play("Scan", tf2);
            }
            //
            tileHolding = null;
        }
    } // End Scan()
    private void Flag()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            Transform target = GameManager.instance.GetTileTransformOnMouse();
            if (target != null)
            {
                eye.Scan();
                Outline(true);
                //
                EffectManager.instance.Fire("Flag",
                    tf, target, flagTime);
            }
        }
    } // End Flag()
    private void Dash()
    {
        if(isDash == false &&  dashCount > 0)
        {
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                dashDir = (mousePos - transform.position).normalized;
                //
                rb.mass = 0.5f;
                rb.AddForce(dashDir * dashSpeed, ForceMode2D.Impulse);
                // 
                if (dashDir.x < 0)
                {
                    rend.flipX = false;
                    eye.FlipX(rend.flipX);
                }
                else if(dashDir.x > 0)
                {
                    rend.flipX = true;
                    eye.FlipX(rend.flipX);
                }
                //
                anim.SetBool(isDashHash, (isDash = true));
                anim.SetTrigger(dashTriggerHash);
                //
                --dashCount;
            }
        }
        else // isDash == true
        {
            if (dashTimeCounter <= 0)
            {
                rb.mass = 1.0f;
                anim.SetBool(isDashHash, (isDash = false));
                dashTimeCounter = dashTime;
                //rb.velocity = Vector2.zero;
            }
            else
            {
                dashTimeCounter -= Time.deltaTime;
                //rb.velocity = dashDir * dashSpeed;
            }
        }
    } // End Dash()
    private void Idle()
    {
        if (!isWalk && !isJump && !isCrouch && !isScan && !isDash)
        {
            idleCounter += Time.deltaTime;
            if (idleCounter > idleTime && isIdle == false)
                eye.SetIdle((isIdle = true));
        }
        else
        {
            idleCounter = 0;
            if (isIdle == true)
                eye.SetIdle((isIdle = false));
        }
    } // End Idle
    private void EyePos()
    {
        if (isDash == true) return;
        //
        if(isJump == true)
        {
            if (rb.velocity.y > 0.1f)
                eye.SetPos(Eye.EyeState.UP);
            else if (rb.velocity.y < -0.1f)
                eye.SetPos(Eye.EyeState.DOWN);
        }
        else
            eye.SetPos(Eye.EyeState.NORMAL);
        //
        if(isCrouch == true)
            eye.SetPos(Eye.EyeState.CROUCH);
    } // End EyePos()
    private void GroundCheck()
    {
        anim.SetFloat(verticalVelocityHash, rb.velocity.y);
        // method_1
        Bounds bounds = col.bounds;
        footPos.x = bounds.center.x;
        footPos.y = bounds.min.y;
        //
        if (rb.velocity.y < -0.01f || rb.velocity.y > 0.01f)
            isJump = true;
        //
        onConcrete = false;
        onFootBoard = false;
        //
        if (Physics2D.OverlapCircle(footPos, footCircleSize,
                GameManager.instance.concreteLayer))
        { // Concrete Check
            onConcrete = true;
            if (rb.velocity.y < -0.1f)
                isJump = false;
        }
        curFootBoard = Physics2D.OverlapCircleAll(footPos, footCircleSize,
                        GameManager.instance.footBoardLayer);
        if (curFootBoard.Length != 0)
        { // FootBoard Check
            onFootBoard = true; // Can Down Jump
            if (rb.velocity.y < -0.1f)
                isJump = false;
        }
        //
        anim.SetBool(isJumpHash, isJump);
    } // End GroundCheck()
    //=========================================//
    private void SetFootBoardTrigger(bool value)
    {
        foreach (Collider2D col in curFootBoard)
        {
            col.isTrigger = true;
            col.usedByEffector = false;
            col.GetComponent<PlatformEffector2D>().enabled = false;
        }
    } // End SetFootBoardTrigger()
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("FootBoard") == true)
        {
            collision.isTrigger = false;
            collision.GetComponent<PlatformEffector2D>().enabled = true;
            collision.usedByEffector = true;
        }
    } // End OnTriggerEnter2D()
} // End of Script Dummy
