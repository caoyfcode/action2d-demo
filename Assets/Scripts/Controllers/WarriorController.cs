using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class WarriorController : MonoBehaviour
{
    internal class Command
    {
        internal float moveX = 0; 
        internal bool jump = false, justJump = false;
        internal bool attack = false;
        internal bool dash = false;
    }

    internal abstract class State
    {
        // Free State: moving x, falling, second jump
        internal abstract class Free: State
        { 
            internal class OnGround : Free { }
            internal class OnAir : Free { }
        }
        internal class Jump: State { }
        internal class Attack: State { }
        internal class Dash: State { }
    }

    const float MAX_RUN_SPEED = 6f;
    const float JUMP_SPEED = 5f;
    const float AIR_JUMP_SPEED = 8f;
    const float MAX_JUMP_TIME = 0.3f;
    const float DASH_TIME = 0.2f;
    const float DASH_SPEED = 18f;
    const float DASH_COOL_DOWN_TIME = DASH_TIME + 0.3f; // 相邻两次 dash 的冷却时间
    const float GRAVITY = 15f;
    const string ANIM_IDLE = "Idle";
    const string ANIM_ATTACK_1 = "Attack_1";
    const string ANIM_ATTACK_2 = "Attack_2";
    const string ANIM_DASH = "Dash";
    const float ATTACK_COMBO_TIME = 0.1f;

    public GameObject smokeRingPrefab;
    Animator animator;
    Rigidbody2D rb;
    BoxCollider2D boxCol;

    // 设置
    public bool enableSecondJump = true;
    public bool enableDash = true;
    

    // 状态
    Vector2 velocity; // 每次 update 使用
    bool Flip
    {
        get => rb.transform.localScale.x < 0;
        set
        {
            float scale = value ? -1 : 1;
            rb.transform.localScale = new Vector3(scale, 1, 1);
        }
    }
    bool _isOnGround;
    bool IsOnGround
    {
        get => _isOnGround;
        set
        {
            animator.SetBool("IsOnGround", value);
            _isOnGround = value;
        }
    }
    
    // 环境检测
    ContactFilter2D envCastFilter;
    RaycastHit2D[] castHits = new RaycastHit2D[5];
    bool isOnLeftWall = false;
    bool isOnRightWall = false;
    bool isOnCeiling = false;

    // 跳跃状态
    // TODO 实现跳跃 coyote time(土狼时间)
    bool isJumping = false;
    Timer jumpTimer = null;
    int airJumpCount = 0; // 在空中跳跃次数, 用于进行二段跳, 三段跳

    // 攻击状态
    public bool isAttacking = false; // set by animator or StopAttack()
    string nextAttackName = ANIM_ATTACK_1;
    bool attackBuffer = false;
    Timer attackComboTimer = null;
    bool prevIsAttacking = false;
    int airAttackCount = 0; // 在空中进行两次攻击后不可再攻击, 直到落地或二段跳

    // 冲刺状态
    bool isDashing = false; // set by StopDash()
    bool isDashCold = false;
    Timer dashTimer = null;
    Timer dashCoolDownTimer = null;


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        boxCol = GetComponent<BoxCollider2D>();
        envCastFilter.SetLayerMask(LayerMask.GetMask("Environment"));
        jumpTimer = new Timer(this);
        attackComboTimer = new Timer(this);
        dashTimer = new Timer(this);
        dashCoolDownTimer = new Timer(this);
    }

    // 从输入获取指令
    Command GetCommand()
    {
        var cmd = new Command
        {
            moveX = Input.GetAxis("Horizontal"),
            justJump = Input.GetButtonDown("Jump"),
            jump = Input.GetButton("Jump"),
            attack = Input.GetButtonDown("Attack"),
            dash = Input.GetButtonDown("Dash"),
        };
        return cmd;
    }

    // 根据分散的状态得到总状态
    State GetState()
    {
        if (isDashing)
        {
            return new State.Dash();
        }
        if (isAttacking)
        {
            return new State.Attack();
        }
        if (isJumping)
        {
            return new State.Jump();
        }
        if (IsOnGround)
        {
            return new State.Free.OnGround();
        }
        else
        {
            return new State.Free.OnAir();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // 更新状态
        isOnLeftWall = boxCol.Cast(Vector2.left, envCastFilter, castHits, 0.2f) > 0;
        isOnRightWall = boxCol.Cast(Vector2.right, envCastFilter, castHits, 0.2f) > 0;
        isOnCeiling = boxCol.Cast(Vector2.up, envCastFilter, castHits, 0.1f) > 0;
        IsOnGround = boxCol.Cast(Vector2.down, envCastFilter, castHits, 0.1f) > 0;
        if (IsOnGround)
        {
            airAttackCount = 0;
            airJumpCount = 0;
        }
        // 获取指令
        var cmd = GetCommand();
        // 聚合状态
        var state = GetState();
        velocity = rb.velocity;

        // 更新状态
        switch (state)
        {
            case State.Free.OnGround:
                if (cmd.dash && enableDash && !isDashCold)
                {
                    Dash();
                    state = new State.Dash();
                }
                else if (cmd.attack || attackBuffer)
                {
                    Attack();
                    state = new State.Attack();
                }
                else if (cmd.justJump)
                {
                    StartJump();
                    state = new State.Jump();
                }
                break;
            case State.Free.OnAir:
                if (cmd.dash && enableDash && !isDashCold)
                {
                    Dash();
                    state = new State.Dash();
                }
                else if ((cmd.attack || attackBuffer) && airAttackCount < 2)
                {
                    Attack();
                    state = new State.Attack();
                }
                else if (cmd.justJump && enableSecondJump && airJumpCount < 1)
                {
                    AirJump();
                    // state does not change
                }
                break;
            case State.Jump:
                if (cmd.dash && enableDash && !isDashCold)
                {
                    StopJump();
                    Dash();
                    state = new State.Dash();
                }
                else if ((cmd.attack || attackBuffer) && airAttackCount < 2)
                {
                    StopJump();
                    Attack();
                    state = new State.Attack();
                }
                else if (!cmd.jump || isOnCeiling)
                {
                    StopJump();
                    state = GetState();
                }
                break;
            case State.Attack:
                if (cmd.dash && enableDash && !isDashCold)
                {
                    StopAttack();
                    Dash();
                    state = new State.Dash();
                }
                else if (cmd.attack)
                {
                    attackBuffer = true;
                }
                break;
            case State.Dash:
                // nothing
                break;
        }

        // 更新额外的动画变量
        animator.SetBool("IsRising", velocity.y > 0);
        animator.SetBool("IsRunning", cmd.moveX != 0 && state is State.Free.OnGround);
        // 处理水平输入的移动
        if (state is State.Jump || state is State.Free) {
            HorizontalMoveAndFlip(cmd.moveX);
        }
        
        if (prevIsAttacking && !isAttacking)
        {
            attackBuffer = false;
            if (nextAttackName == ANIM_ATTACK_2)
            {
                attackComboTimer.Reset(ATTACK_COMBO_TIME, () => nextAttackName = ANIM_ATTACK_1);
            }
        }
        prevIsAttacking = isAttacking;
        rb.velocity = velocity;
    }

    void FixedUpdate()
    {
        if (!isAttacking && !IsOnGround && !isJumping && !isDashing)
        {
            var velocity = rb.velocity;
            velocity.y -= GRAVITY * Time.deltaTime;
            rb.velocity = velocity;
        }
    }

    void HorizontalMoveAndFlip(float moveX)
    {
        velocity.x = 0;
        if (moveX != 0)
        {
            Flip = moveX < 0;
            if ((moveX < 0 && isOnLeftWall) || (moveX > 0 && isOnRightWall))
            {
                velocity.x = 0;
            }
            else
            {
                velocity.x = moveX * MAX_RUN_SPEED;
            }
        }
    }

    // 转换到 Attack 状态
    void Attack()
    {
        animator.Play(nextAttackName);
        velocity.x = 0;
        velocity.y = 0;
        nextAttackName = nextAttackName == ANIM_ATTACK_1 ? ANIM_ATTACK_2 : ANIM_ATTACK_1;
        attackBuffer = false;
        if (!IsOnGround)
        {
            airAttackCount++;
        }
    }

    // attack 被打断(被dash或者受伤打断)
    void StopAttack()
    {
        animator.Play(ANIM_IDLE);
        isAttacking = false;
        attackComboTimer.Stop();
        nextAttackName = ANIM_ATTACK_1;
        attackBuffer = false;
        airAttackCount = 0;
    }

    void StartJump()
    {
        velocity.y = JUMP_SPEED;
        isJumping = true;
        jumpTimer.Reset(MAX_JUMP_TIME, () => isJumping = false);
    }

    void StopJump()
    {
        velocity.y = 0;
        isJumping = false;
        jumpTimer.Stop();
    }

    void AirJump()
    {
        velocity.y = AIR_JUMP_SPEED;
        isJumping = false;
        airJumpCount++;
        airAttackCount = 0; // 跳跃重置攻击
        Instantiate(smokeRingPrefab, new Vector3(transform.position.x - 0.1f, transform.position.y - 0.7f, 0), Quaternion.identity);
    }

    void Dash()
    {
        animator.Play(ANIM_DASH);
        velocity.y = 0;
        velocity.x = DASH_SPEED * (Flip ? -1 : 1);
        isDashing = true;
        isDashCold = true;
        airAttackCount = 0; // 冲刺重置攻击
        dashTimer.Reset(DASH_TIME, () => StopDash());
        dashCoolDownTimer.Reset(DASH_COOL_DOWN_TIME, () => isDashCold = false);
    }

    void StopDash()
    {
        animator.Play(ANIM_IDLE);
        velocity.y = 0;
        velocity.x = 0;
        isDashing = false;
        dashTimer.Stop();
    }
}
