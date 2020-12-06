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
    // Idle
    public bool isIdle;
    public float idleCounter;
    public float idleTime;
    // Component
    Transform tf;
    SpriteRenderer rend;
    Animator anim;
    Rigidbody2D rb;
    BoxCollider2D col;
    // Collider
    public enum ColState { NORMAL, CROUCH };
    Vector2[] colOffset = new Vector2[2]; // 0:normal, 1:crouch
    Vector2[] colSize = new Vector2[2]; // 0:normal, 1:crouch
    // Eye
    Eye eye;
    // FootBoard now on
    public Collider2D onBoard;

    //////////////////////////////////////////////////// 

    void Start()
    {
        moveSpeed = 1.0f;
        jumpForce = 6.5f;
        idleTime = 5.0f;
        //
        tf = GetComponent<Transform>();
        rend = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        //
        colOffset[0] = new Vector2(0, +0.02f);
        colOffset[1] = new Vector2(0, -0.04f);
        colSize[0] = new Vector2(0.52f, 1.12f);
        colSize[1] = new Vector2(0.52f, 1.00f);
        //
        eye = transform.GetChild(0).GetComponent<Eye>();
        eye.dummy = this;
    } // End Start

    void Update()
    {
        Control();
    } // End Update

    void FixedUpdate()
    {
        Move();
        Idle();
    } // End FixedUpdate

    //////////////////////////////////////////////////// 

    void Control()
    {
        moveVelocity.x = Input.GetAxisRaw("Horizontal");

        Jump();
        Crouch();
        Scan();
    } // End Control
    
    void Move()
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
        transform.Translate(moveVelocity * moveSpeed * Time.deltaTime);
    } // End Move

    void Jump()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (isJump == true)
                return;

            anim.SetBool("isJump", (isJump = true));
            //
            if (isCrouch == true)
            {
                onBoard.isTrigger = true;
                anim.SetBool("isCrouch", (isCrouch = false));
            }
            else
            {
                eye.SetPos(Eye.EyeState.UP);
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }
            //
        }
        //
        if (isJump == true)
        {
            anim.SetFloat("verticalVelocity", rb.velocity.y);
            //
            if (rb.velocity.y < 0)
                eye.SetPos(Eye.EyeState.DOWN);
        }
    } // End Jump

    void Crouch()
    {
        if (Input.GetKey(KeyCode.S))
        {
            if (isJump == true)
                return;
            //
            if (isCrouch == true)
                return;

            // Set Anim
            anim.SetBool("isCrouch", (isCrouch = true));
            anim.SetBool("isWalk", (isWalk = false));
            // Set Eye
            eye.SetPos(Eye.EyeState.CROUCH);
            // Set Collider
            col.offset = colOffset[(int)ColState.CROUCH];
            col.size = colSize[(int)ColState.CROUCH];
        }

        if (Input.GetKeyUp(KeyCode.S))
        {
            // Set Anim
            anim.SetBool("isCrouch", (isCrouch = false));
            // Set Eye
            eye.SetPos(Eye.EyeState.NORMAL);
            // Set Collider
            col.offset = colOffset[(int)ColState.NORMAL];
            col.size = colSize[(int)ColState.NORMAL];
        }
    } // End Crouch

    void Scan()
    {
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            isScan = true;
            eye.Scan();
        }

    } // End Scan

    void Idle()
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

    void OnGround()
    {
        if (isJump == false)
            return;
        // Flag
        anim.SetBool("isJump", (isJump = false));
        // Body
        rb.velocity = Vector3.zero;
        anim.SetFloat("verticalVelocity", rb.velocity.y);
        // Eye
        if (isCrouch)
            eye.SetPos(Eye.EyeState.CROUCH);
        else
            eye.SetPos(Eye.EyeState.NORMAL);
    } // End OnGround

    private void OnCollisionExit2D(Collision2D collision)
    { // 점프할 때 FootBoard Trigger Set
        if (collision.transform.tag == "FootBoard")
        {
            onBoard = null;
            float footBoardPosY = collision.transform.position.y - 1.0f;
            if (tf.position.y > footBoardPosY)
            {
                collision.collider.isTrigger = true;
            }
        }
    } // End OnCollisionExit2D

    private void OnTriggerEnter2D(Collider2D collision)
    { // 점프할 때 Dummy <-> FootBoard 상대 위치에 따라 Trigger Set
        if (collision.transform.tag == "FootBoard")
        {
            float footBoardPosY = collision.transform.position.y - 1.0f;
            if (tf.position.y > footBoardPosY)
            {
                onBoard = collision.GetComponent<Collider2D>();
                collision.isTrigger = false;
                OnGround();
            }
        }
    } // End OnTriggerEnter2D

} // End of Script Dummy
