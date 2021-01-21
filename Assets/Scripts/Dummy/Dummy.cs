using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : MonoBehaviour
{
    // Physics
    public float moveSpeed;
    public float jumpForce;
    Vector2 moveVelocity;
    // BodyFlags
    public bool isWalk;
    public bool isJump;
    public bool isCrouch;
    public bool isScan;
    public bool onFootBoard;
    // Idle
    public bool isIdle;
    public float idleCounter;
    public float idleTime;
    // Component
    private Transform tf;
    private SpriteRenderer rend;
    private Animator anim;
    private Rigidbody2D rb;
    private BoxCollider2D col;
    // Collider
    public enum ColState { NORMAL, CROUCH };
    private Vector2[] colOffset = new Vector2[2]; // 0:normal, 1:crouch
    private Vector2[] colSize = new Vector2[2]; // 0:normal, 1:crouch
    // Eye
    private Eye eye;
    // GroundCheck
    Vector2 footPos = Vector2.zero;
    float footCircleSize = 5.0f;
    private int footBoardLayer;
    private int concreteLayer;
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
        footBoardLayer = LayerMask.GetMask("FootBoard");
        concreteLayer = LayerMask.GetMask("Concrete");
    } // End Start
    //=========================================//
    private void Update()
    {
        Control();
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
    private void Control()
    {
        Walk();
        Jump();
        Crouch();
        Scan();
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
            anim.SetBool("isWalk", (isWalk = true));
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
            anim.SetBool("isWalk", (isWalk = false));
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
                if(onFootBoard == true)
                    col.isTrigger = true;
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
            anim.SetBool("isCrouch", (isCrouch = true));
            // Set Collider
            col.offset = colOffset[(int)ColState.CROUCH];
            col.size = colSize[(int)ColState.CROUCH];
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            // Set Anim
            anim.SetBool("isCrouch", (isCrouch = false));
            // Set Collider
            col.offset = colOffset[(int)ColState.NORMAL];
            col.size = colSize[(int)ColState.NORMAL];
        }
    } // End Crouch
    private void Scan()
    {
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            isScan = true;
            eye.Scan();
        }
    } // End Scan
    private void Idle()
    {
        if (!isWalk && !isJump && !isCrouch && !isScan)
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
        if(rb.velocity.y > 0.1f)
            eye.SetPos(Eye.EyeState.UP);
        else if(rb.velocity.y < -0.1f)
            eye.SetPos(Eye.EyeState.DOWN);
        else
            eye.SetPos(Eye.EyeState.NORMAL);
        //
        if(isCrouch == true)
            eye.SetPos(Eye.EyeState.CROUCH);
    } // End EyePos()
    private void GroundCheck()
    {
        anim.SetFloat("verticalVelocity", rb.velocity.y);
        // method_1
        Bounds bounds = col.bounds;
        footPos.x = bounds.center.x;
        footPos.y = bounds.min.y;
        //
        if (-0.1f < rb.velocity.y && rb.velocity.y < 0.1f)
        {
            if(Physics2D.OverlapCircle(footPos, footCircleSize, concreteLayer))
                isJump = false;
            //
            else if (Physics2D.OverlapCircle(footPos, footCircleSize, footBoardLayer))
            {
                onFootBoard = true; // Can Down Jump
                isJump = false;
            }
        }
        else
        {
            isJump = true;
            onFootBoard = false;
        }
        //
        anim.SetBool("isJump", isJump);
        /*
        // method_2
        if (rb.velocity.y < -0.1f || rb.velocity.y > 0.1f)
            anim.SetBool("isJump", (isJump = true));
        if (rb.velocity.y == 0)
            anim.SetBool("isJump", (isJump = false));
        */
    }
    //=========================================//
    private void OnTriggerExit2D(Collider2D collision)
    {
        col.isTrigger = false;
    } // End OnTriggerEnter2D
} // End of Script Dummy
