using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warrior : MonoBehaviour
{
    #region Constant Number
    internal const float MAX_RUN_SPEED = 6f;
    internal const float JUMP_SPEED = 5f;
    internal const float AIR_JUMP_SPEED = 8f;
    internal const float MAX_JUMP_TIME = 0.3f;
    internal const float DASH_TIME = 0.2f;
    internal const float DASH_SPEED = 18f;
    internal const float DASH_COOL_DOWN_TIME = DASH_TIME + 0.3f; // 相邻两次 dash 的冷却时间
    internal const float WALL_SLIDE_SPEED = 2f;
    const float GRAVITY = 15f;
    internal const float ATTACK_COMBO_TIME = 0.1f;
    internal const string ANIM_IDLE = "Idle";
    internal const string ANIM_RUN = "Run";
    internal const string ANIM_RISE = "Rise";
    internal const string ANIM_FALL = "Fall";
    internal const string ANIM_ATTACK_1 = "Attack_1";
    internal const string ANIM_ATTACK_2 = "Attack_2";
    internal const string ANIM_DASH = "Dash";
    internal const string ANIM_WALL_SLIDE = "WallSlide";
    internal const string ANIM_WALL_STAY = "WallStay";
    #endregion

    #region Component
    public GameObject smokeRingPrefab;
    internal Animator animator;
    private Rigidbody2D rb;
    private BoxCollider2D boxCol;
    #endregion

    #region Abilitiy
    [Header("Ability")]
    public bool secondJumpAbility = true;
    public bool dashAbility = true;
    public bool wallSlideAbility = true;
    #endregion

    #region Command
    public class Command
    {
        public float moveX = 0; 
        public bool holdJump = false, startJump = false;
        public bool attack = false;
        public bool dash = false;
        public bool slideDown = false;
    }
    internal Command CurrentCommand {get; private set;}
    private bool gotNewCommand = false;
    #endregion

    #region Status
    internal Vector2 velocity; // 每次 update 期间使用
    private float _facingDirection = 1;
    internal float FacingDirection // 1 向右, -1 向左
    {
        get => _facingDirection;
        private set
        {   if (value * _facingDirection < 0)
            {
                rb.transform.Rotate(0, 180, 0);
                _facingDirection = -_facingDirection;
            }
        }
    }
    internal bool isOnGround = false;
    internal bool isOnCeiling = false;
    internal bool isOnLeftWall = false;
    internal bool isOnRightWall = false;
    // 之所以两边都检测是因为更新过程方向会变
    internal bool IsFacingWall => FacingDirection > 0 ? isOnRightWall : isOnLeftWall;
    internal int airJumpCount = 0;
    internal int airAttackCount = 0;
    internal int airDashCount = 0;
    public bool attackCancelable;
    public bool attackEnd;
    public bool hitBoxEnabled;
    internal float dashCoolDownTimer = 0;
    internal float attackComboTimer = 0;
    #endregion

    #region State Machine
    internal StateMachine<WarriorState> stateMachine;
    internal WarriorGroundFree groundFreeState;
    internal WarriorAirFree airFreeState;
    internal WarriorHoldJump holdJumpState;
    internal WarriorAttack attackState;
    internal WarriorDash dashState;
    internal WarriorWallSlide wallSlideState;
    #endregion

    private ContactFilter2D envCastFilter;
    private RaycastHit2D[] castHits = new RaycastHit2D[5];

    public void AddCommand(Command cmd)
    {
        CurrentCommand = cmd;
        gotNewCommand = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        boxCol = GetComponent<BoxCollider2D>();
        envCastFilter.SetLayerMask(LayerMask.GetMask("Environment"));
        stateMachine = new StateMachine<WarriorState>();
        groundFreeState = new WarriorGroundFree(this, stateMachine);
        airFreeState = new WarriorAirFree(this, stateMachine);
        holdJumpState = new WarriorHoldJump(this, stateMachine);
        attackState = new WarriorAttack(this, stateMachine);
        dashState = new WarriorDash(this, stateMachine);
        wallSlideState = new WarriorWallSlide(this, stateMachine);
        stateMachine.Init(groundFreeState);
    }

    // Update is called once per frame
    void Update()
    {
        // 本帧的状态
        isOnGround = boxCol.Cast(Vector2.down, envCastFilter, castHits, 0.1f) > 0;
        isOnCeiling = boxCol.Cast(Vector2.up, envCastFilter, castHits, 0.1f) > 0;
        isOnRightWall = boxCol.Cast(Vector2.right, envCastFilter, castHits, 0.1f) > 0;
        isOnLeftWall = boxCol.Cast(Vector2.left, envCastFilter, castHits, 0.1f) > 0;
        if (!gotNewCommand)
        {
            CurrentCommand = new Command();
        }
        velocity = rb.velocity;
        UpdateTimers();
        stateMachine.Update();
        gotNewCommand = false;
        rb.velocity = velocity;
    }

    void FixedUpdate()
    {
        if (stateMachine.CurrentState == airFreeState)
        {
            var velocity = rb.velocity;
            velocity.y -= GRAVITY * Time.deltaTime;
            rb.velocity = velocity;
        }
        // 遇到墙不再前进，避免与墙重叠导致错误检测到地面
        if (IsFacingWall && rb.velocity.x * FacingDirection > 0)
        {
            var velocity = rb.velocity;
            velocity.x = 0;
            rb.velocity = velocity;
        }
    }

    void UpdateTimers()
    {
        if (dashCoolDownTimer > 0)
            dashCoolDownTimer -= Time.deltaTime;
        if (attackComboTimer > 0)
            attackComboTimer -= Time.deltaTime;
    }

    public void HorizontalMoveAndFlip(float moveX)
    {
        velocity.x = 0;
        if (moveX != 0)
        {
            FacingDirection = moveX > 0 ? 1 : -1;
            velocity.x = moveX * MAX_RUN_SPEED;
        }
    }
}
