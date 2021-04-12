﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : MonoBehaviour
{
    //===============================//
    public bool isInvincible = false;
    //===============================//
    public bool isDead = false;
    private List<string> deadTags = new List<string>()
        { "Explosion", "Thunder", "Laser" };
    //Debug
    float R_LeftBorder;
    float L_RightBorder;
    // Physics //
    // move
    public float moveSpeed;
    Vector2 moveVelocity;
    // Jump
    public float jumpForce;
    // GroundCheck
    public Collider2D[] curFootBoard;
    // JumpPack
    private readonly int jumpPackAnimLayer = 1;
    //===============================//
    // bool Flags //
    public bool isWalk;
    public bool isJump;
    public bool isCrouch;
    public bool usingSkill;
    public bool noRSC;
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
    private SpriteRenderer sr;
    [HideInInspector]
    public Animator anim;
    private Rigidbody2D rb;
    private BoxCollider2D col;
    //===============================//
    // Collider
    public enum ColState { NORMAL, CROUCH };
    private Vector2[] colOffset = new Vector2[2]; // 0:normal, 1:crouch
    private Vector2[] colSize = new Vector2[2]; // 0:normal, 1:crouch
    //===============================//
    // Child Object
    // Eye
    private Eye eye;
    // Arm
    public Arm arm;
    // ShieldEffect
    private ShieldEffect shieldEffect;
    public ShieldEffect.ShieldState shieldState;
    //===============================//
    // GroundCheck
    private Vector2 footPos = new Vector2(0, -13.0f);
    private Vector2 footBoxSize = new Vector2(10.0f, 5.0f);
    //
    //=========================================//
    // Outline
    [ColorUsageAttribute(true, true)]
    public Color shieldNormalColor;
    private float shieldNormalThickness = 1.0f;
    [ColorUsageAttribute(true, true)]
    public Color shieldActiveColor;
    private float shieldActiveThickness = 2.0f;
    //=========================================//
    // Scan : HoldTile
    private Transform tileHolding;
    public int scanCost = 3;
    public int flagCost = 30;
    //=========================================//
    // anim Hash
    private readonly int isJumpHash = Animator.StringToHash("isJump");
    private readonly int isWalkHash = Animator.StringToHash("isWalk");
    private readonly int isCrouchHash = Animator.StringToHash("isCrouch");
    private readonly int isDowningHash = Animator.StringToHash("isDowning");
    private readonly int verticalVelocityHash = Animator.StringToHash("verticalVelocity");
    //
    private readonly int landJumpHash = Animator.StringToHash("landJump");
    private readonly int airJumpHash = Animator.StringToHash("airJump");
    // shader Hash
    //private readonly int hashOutlineColorInner = Shader.PropertyToID("_OutlineColor_Inner");
    //private readonly int hashOutlineTK = Shader.PropertyToID("_OutlineThickness");
    //=========================================//
    private void Start()
    {
        tf = GetComponent<Transform>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        //
        colOffset[0] = new Vector2(0, 0);
        colOffset[1] = new Vector2(0, -1.5f);
        colSize[0] = new Vector2(13.0f, 28.0f);
        colSize[1] = new Vector2(13.0f, 25.0f);
        //
        eye = tf.GetChild(0).GetComponent<Eye>();
        eye.dummy = this;
        //
        arm = tf.GetChild(1).GetComponent<Arm>();
        arm.dummy = this;
        //
        shieldEffect = tf.GetChild(2).GetComponent<ShieldEffect>();
        shieldEffect.dummy = this;
    }
    //=========================================//
    private void Update()
    {
        if (GameManager.instance.pause == true)
            return;
        //
        Control();
        //
        UpdateEyeState();
        //
        GroundCheck();
        //
        Move();
        //
        Idle();
        //
        DebugPlayer();
    } // End Update
    //private void FixedUpdate()
    //{
    //    Move();
    //    Idle();
    //} // End FixedUpdate
    private void LateUpdate()
    {
        StuckInConcrete();
    }
    //=========================================//
    private void DebugPlayer()
    {
        if (Input.GetKeyDown(KeyCode.End))
            GameManager.instance.ClearStage();
        else if (Input.GetKeyDown(KeyCode.F5))
        {
            isInvincible = !isInvincible;
            Debug.Log("Invincible : " + isInvincible);
        }

    }
    //
    /*
    private void OnDrawGizmos()
    {
        if (onConcrete || onFootBoard)
            Gizmos.color = Color.black;
        else
            Gizmos.color = Color.green;
        Gizmos.DrawCube(footPos, footBoxSize);
        //
        {
            Vector3 from = new Vector3(R_LeftBorder, 0, 0);
            Vector3 to = new Vector3(R_LeftBorder, 400, 0);
            Gizmos.DrawLine(from, to);
        }
        {
            Vector3 from = new Vector3(L_RightBorder, 0, 0);
            Vector3 to = new Vector3(L_RightBorder, 400, 0);
            Gizmos.DrawLine(from, to);
        }
    }
    */
    //=========================================//
    private void StuckInConcrete()
    {
        float rayDist = 64.0f;
        float dummyColHalfX = colSize[0].x * 0.5f;
        float concreteColHalf_X = 30.5f;
        float dist = 0;
        //
        RaycastHit2D hit_R = Physics2D.Raycast(tf.position, Vector3.right,
            rayDist, GameManager.instance.concreteLayer);
        RaycastHit2D hit_L = Physics2D.Raycast(tf.position, Vector3.left,
            rayDist, GameManager.instance.concreteLayer);
        //
        Vector3 temp = tf.position;
        if (hit_R)
        {
            R_LeftBorder = hit_R.collider.transform.position.x - concreteColHalf_X;
            dist = Mathf.Abs(R_LeftBorder - tf.position.x);
            if (dist <= dummyColHalfX)
            {
                temp.x = R_LeftBorder - dummyColHalfX;
                tf.position = temp;
            }
        }
        if (hit_L)
        {
            L_RightBorder = hit_L.collider.transform.position.x + concreteColHalf_X;
            dist = Mathf.Abs(L_RightBorder - tf.position.x);
            if (dist <= dummyColHalfX)
            {
                temp.x = L_RightBorder + dummyColHalfX;
                tf.position = temp;
            }
        }
        if (hit_R && hit_L)
        {
            float gap = R_LeftBorder - L_RightBorder;
            //Debug.Log("gap : " + gap);
            if (gap <= (colSize[0].x + 2.0f))
            {
                hit_R.collider.transform.GetComponent<SpriteRenderer>().color = Color.red;
                hit_L.collider.transform.GetComponent<SpriteRenderer>().color = Color.red;
                //
                Dead("Crash");
                tf.gameObject.SetActive(false);
            }
        }
    }
    //=========================================//
    public void ActiveShield()
    { // When Hit
        //if (UIManager.instance.shieldCount <= 0)
        //    return;
        if (shieldState == ShieldEffect.ShieldState.NONE
            || shieldState == ShieldEffect.ShieldState.ACTIVE)
            return;
        //
        UIManager.instance.ActiveShield();
        //
        shieldEffect.Activate();
        //
        Outline(ShieldEffect.ShieldState.ACTIVE);
    }
    //
    public void Outline(ShieldEffect.ShieldState state)
    {
        shieldState = state;
        //
        switch (state)
        {
            case ShieldEffect.ShieldState.NONE:
                {
                    foreach (var ol in GetComponentsInChildren<Outline>())
                        ol.SetOutline(shieldNormalColor, 0);
                }
                break;
            case ShieldEffect.ShieldState.NORMAL:
                {
                    foreach (var ol in GetComponentsInChildren<Outline>())
                        ol.SetOutline(shieldNormalColor, shieldNormalThickness);
                }
                break;
            case ShieldEffect.ShieldState.ACTIVE:
                {
                    foreach (var ol in GetComponentsInChildren<Outline>())
                        ol.SetOutline(shieldActiveColor, shieldActiveThickness);
                }
                break;
            default:
                break;
        }
        /*
        if (state == ShieldEffect.ShieldState.ACTIVE)
        {
            sr.material.SetColor(hashOutlineColorInner, shieldActiveColor);
            sr.material.SetFloat(hashOutlineTK, shieldActiveThickness);
        }
        else if (state == ShieldEffect.ShieldState.NORMAL)
        {
            sr.material.SetColor(hashOutlineColorInner, shieldNormalColor);
            sr.material.SetFloat(hashOutlineTK, shieldNormalThickness);
        }
        else // if (state == ShieldEffect.ShieldState.NONE)
        {
            sr.material.SetFloat(hashOutlineTK, 0);
        }
        //EffectManager.instance.SetBloom(value);
        */
    }
    //=========================================//
    public void SetJumpPackLayer(bool value)
    {
        anim.SetLayerWeight(jumpPackAnimLayer, System.Convert.ToSingle(value));
    }
    //=========================================//
    public void SetIsDown()
    { // Anim Frame Event
        anim.SetBool(isDowningHash, true);
    }
    //=========================================//
    private void Control()
    {
        if (GameManager.instance.pause)
            return;
        //
        Walk();     // 'WASD'
        Jump();     // /Space
        Crouch();   // 'S'
        Scan();     // M_LBtn
        Flag();     // M_RBtn
    }
    //
    private void Walk()
    {
        moveVelocity.x = Input.GetAxisRaw("Horizontal");
    }
    //
    private void Flips(bool value)
    {
        sr.flipX = value;
        eye.FlipX(value);
        arm.FlipX(value);
    }
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
            Flips(moveVelocity.x > 0);
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
        if (isJump)
        {
            if (UIManager.instance.airJumpCount <= 0)
                return;
        }
        //
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isCrouch == true) // Down Jump
            {
                if (onFootBoard == true)
                    SetFootBoardTrigger(true);
            }
            else
            { // Land Jump
                anim.SetBool(isDowningHash, false);
                //
                if (isJump)
                {
                    rb.velocity = Vector3.zero;
                    AirJump();
                }
                else
                    anim.SetTrigger(landJumpHash);
                //
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }
        }
    }
    //
    private void AirJump()
    {
        anim.SetTrigger(airJumpHash);
        //
        UIManager.instance.UseAirJump();
        //
        arm.UseJumpPack();
    }
    //
    private void Crouch()
    {
        if (Input.GetKey(KeyCode.S))
        {
            if ((isJump == true) || (isCrouch == true))
                return;
            // Stop Walk
            anim.SetBool(isWalkHash, (isWalk = false));
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
    }
    //
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
            Transform target = GameManager.instance.GetTileTransformOnMouse();
            if (target == null)
                return;
            //
            if (target == tileHolding)
            {
                if (UIManager.instance.UseRSC(scanCost) == false)
                {
                    NoRSC();
                    return;
                }
                //
                usingSkill = true;
                eye.PlayEyeSkillAnim();
                //
                target.GetComponent<Closet>().Flip();
                //
                EffectManager.instance.Play("Scan", target);
            }
            //
            tileHolding = null;
        }
    } // End Scan()
    private void Flag()
    {
        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            if (UIManager.instance.UseRSC(flagCost) == false)
            {
                NoRSC();
                return;
            }
            //
            Transform target = GameManager.instance.GetTileTransformOnMouse();
            if (target != null)
            {
                eye.PlayEyeSkillAnim();
                //
                target.GetComponent<Closet>().HitFlag();
                EffectManager.instance.Play("Flag", target);
            }
        }
    }
    //
    private void NoRSC()
    {
        noRSC = true;
        eye.PlayEyeNoRSCAnim();
        Debug.Log("자원이 부족합니다.");
    }
    //
    private void Idle()
    {
        if (!isWalk && !isJump && !isCrouch && !usingSkill)
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
    }
    //
    private void UpdateEyeState()
    {
        if (isJump == true)
        {
            if (rb.velocity.y > 0.1f)
                eye.SetState(Eye.EyeState.UP);
            else if (rb.velocity.y < -0.1f)
                eye.SetState(Eye.EyeState.DOWN);
        }
        else
        {
            if(isCrouch == false)
                eye.SetState(Eye.EyeState.NORMAL);
            if(isCrouch == true)
                eye.SetState(Eye.EyeState.CROUCH);
        }
    }
    //
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
        if (Physics2D.OverlapBox(footPos, footBoxSize, 0,
                GameManager.instance.concreteLayer))
        { // Concrete Check
            onConcrete = true;
            isJump = false;
        }
        curFootBoard = Physics2D.OverlapBoxAll(footPos, footBoxSize, 0,
                        GameManager.instance.footBoardLayer);
        if (curFootBoard.Length != 0)
        { // FootBoard Check
            if (curFootBoard[0].CompareTag("FootBoard"))
            {
                onFootBoard = true; // Can Down Jump
                if (rb.velocity.y <= 0)
                    isJump = false;
            }
            else if (curFootBoard[0].CompareTag("LastFootBoard"))
            {
                onFootBoard = true; // Can Down Jump
                if (rb.velocity.y <= 0)
                {
                    isJump = false;
                    // Start Clear Sequence
                    GameManager.instance.ClearStage();
                }
            }
        }
        //
        anim.SetBool(isJumpHash, isJump);
        //
        if(isJump == false)
            anim.SetBool(isDowningHash, false);
    }
    //=========================================//
    private void SetFootBoardTrigger(bool value)
    {
        foreach (Collider2D col in curFootBoard)
        {
            col.isTrigger = true;
            col.usedByEffector = false;
            //
            //PlatformEffector2D effector = null;
            //if (col.TryGetComponent<PlatformEffector2D>(out effector))
            //    effector.enabled = false;
            col.GetComponent<PlatformEffector2D>().enabled = false;
        }
    }
    //
    public void Dead(string tag)
    {
        if (isDead || isInvincible)
            return;
        //
        switch (shieldState)
        {
            case ShieldEffect.ShieldState.NONE:
                break;
            case ShieldEffect.ShieldState.NORMAL:
                if (tag != "Concrete")
                {
                    ActiveShield();
                    return;
                }
                break;
            case ShieldEffect.ShieldState.ACTIVE:
                return;
            default:
                break;
        }
        //
        // Dead Sequance
        Debug.Log("Player Dead By [" + tag + "]");
        isDead = true;
        GameManager.instance.gameOver = true;
        //
        string record = GameManager.instance.elecShooter.superViser.currentFloor.ToString();
        SceneControl.instance.saveData.bestRecord = record;
        //
        gameObject.SetActive(false);
        //
        GameManager.instance.ReloadPlayScene();
    }
    //
    private void OnTriggerEnter2D(Collider2D collision)
    {
        foreach (string tag in deadTags)
            if (collision.CompareTag(tag) == true)
                Dead(tag);
    }
    //
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("FootBoard") == true)
        {
            collision.isTrigger = false;
            //
            //PlatformEffector2D effector = null;
            //if (col.TryGetComponent<PlatformEffector2D>(out effector))
            //    effector.enabled = true;
            collision.GetComponent<PlatformEffector2D>().enabled = true;
            collision.usedByEffector = true;
        }
    }
    //
} // End of Script Dummy
