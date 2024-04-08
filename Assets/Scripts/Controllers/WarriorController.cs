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
        // internal bool dash = false;
    }

    internal abstract class State
    {
        // Free State: moving x, falling 
        internal abstract class Free: State
        { 
            internal class OnGround : Free { }
            internal class OnAir : Free { }
        }
        internal class Jump: State { }
        internal class Attack: State { }
    }

    const float MAX_RUN_SPEED = 6f;
    const float JUMP_SPEED = 5f;
    const float MAX_JUMP_TIME = 0.3f;
    const float DASH_TIME = 0.2f;
    const float DASH_SPEED = 18f;
    const float GRAVITY = 15f;
    const string ANIM_ATTACK_1 = "Attack_1";
    const string ANIM_ATTACK_2 = "Attack_2";
    const float ATTACK_COMBO_TIME = 0.1f;

    Animator animator;
    Rigidbody2D rb;
    BoxCollider2D boxCol;

    ContactFilter2D envCastFilter; // 环境检测
    RaycastHit2D[] castHits = new RaycastHit2D[5];
    

    // 状态
    bool IsAttacking => animator.GetBool("IsAttacking");
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
    bool _isJumping;
    bool IsJumping
    {
        get => _isJumping;
        set
        {
            animator.SetBool("IsJumping", value);
            _isJumping = value;
        }
    }
    bool isOnLeftWall = false;
    bool isOnRightWall = false;

    // 跳跃状态
    // TODO 实现跳跃 coyote time(土狼时间)
    Timer jumpTimer = null;

    // 攻击状态
    string nextAttack = ANIM_ATTACK_1;
    bool attackBuffer = false;
    Timer attackComboTimer = null;
    bool prevIsAttacking = false;
    int airAttackCount = 0; // 在空中进行两次攻击后不可再攻击, 直到落地或二段跳


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        boxCol = GetComponent<BoxCollider2D>();
        envCastFilter.SetLayerMask(LayerMask.GetMask("Environment"));
        jumpTimer = new Timer(this);
        attackComboTimer = new Timer(this);
    }

    // 从输入获取指令
    Command GetCommand()
    {
        var cmd = new Command
        {
            moveX = Input.GetAxis("Horizontal"),
            justJump = Input.GetButtonDown("Jump"),
            jump = Input.GetButton("Jump"),
            attack = Input.GetButtonDown("Attack")
        };
        return cmd;
    }

    // 根据分散的状态得到总状态
    State GetState()
    {
        if (IsAttacking)
        {
            return new State.Attack();
        }
        if (IsJumping)
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
        IsOnGround = boxCol.Cast(Vector2.down, envCastFilter, castHits, 0.1f) > 0;
        if (IsOnGround)
        {
            airAttackCount = 0;
        }
        // 获取指令
        var cmd = GetCommand();
        // 聚合状态
        var state = GetState();
        var velocity = rb.velocity;

        // 更新状态
        switch (state)
        {
            case State.Free.OnGround:
                if (cmd.attack || attackBuffer)
                {
                    Attack(out velocity);
                    state = new State.Attack();
                }
                else if (cmd.justJump)
                {
                    StartJump(ref velocity);
                    state = new State.Jump();
                }
                break;
            case State.Free.OnAir:
                if ((cmd.attack || attackBuffer) && airAttackCount < 2)
                {
                    Attack(out velocity);
                    state = new State.Attack();
                }
                break;
            case State.Jump:
                if (!cmd.jump)
                {
                    StopJump(ref velocity);
                    state = GetState();
                }
                break;
            case State.Attack:
                if (cmd.attack)
                {
                    attackBuffer = true;
                }
                break;
        }

        // 额外处理水平移动
        animator.SetBool("IsRunning", cmd.moveX != 0 && state is State.Free.OnGround);
        velocity.x = 0;
        if (state is State.Jump || state is State.Free)
        {
            if (cmd.moveX != 0)
            {
                Flip = cmd.moveX < 0;
                if ((cmd.moveX < 0 && isOnLeftWall) || (cmd.moveX > 0 && isOnRightWall))
                {
                    velocity.x = 0;
                }
                else
                {
                    velocity.x = cmd.moveX * MAX_RUN_SPEED;
                }
            }
        }

        if (nextAttack == ANIM_ATTACK_2 && prevIsAttacking == true && IsAttacking == false)
        {
            attackComboTimer.Reset(ATTACK_COMBO_TIME, () => nextAttack = ANIM_ATTACK_1);
        }
        prevIsAttacking = IsAttacking;
        rb.velocity = velocity;
    }

    void FixedUpdate()
    {
        if (!IsAttacking && !IsOnGround && !IsJumping)
        {
            var velocity = rb.velocity;
            velocity.y -= GRAVITY * Time.deltaTime;
            rb.velocity = velocity;
        }
    }

    // 转换到 Attack 状态
    void Attack(out Vector2 velocity)
    {
        animator.Play(nextAttack);
        velocity.x = 0;
        velocity.y = 0;
        StopJump(ref velocity);
        nextAttack = nextAttack == ANIM_ATTACK_1 ? ANIM_ATTACK_2 : ANIM_ATTACK_1;
        attackBuffer = false;
        if (!IsOnGround)
        {
            airAttackCount++;
        }
    }

    void StartJump(ref Vector2 velocity)
    {
        velocity.y = JUMP_SPEED;
        IsJumping = true;
        jumpTimer.Reset(MAX_JUMP_TIME, () => IsJumping = false);
    }

    void StopJump(ref Vector2 velocity)
    {
        velocity.y = 0;
        IsJumping = false;
        jumpTimer.Stop();
    }
}
