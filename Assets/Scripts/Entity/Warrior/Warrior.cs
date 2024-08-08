using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warrior : Entity
{
    #region Constant Number
    internal float jumpSpeed = 5f;
    internal float airJumpSpeed = 8f;
    internal float maxJumpTime = 0.3f;
    internal float dashTime = 0.2f;
    internal float dashSpeed = 18f;
    internal float dashCoolDownTime = 0.5f; // 0.2+0.3 相邻两次 dash 的冷却时间
    internal float wallSlideSpeed = 2f;
    internal float attackComboTime = 0.1f;
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
    [Header("Component")]
    public GameObject smokeRingPrefab;
    public Collider2D hitBox;
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
    internal int airJumpCount = 0;
    internal int airAttackCount = 0;
    internal int airDashCount = 0;
    [Header("Status")]
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

    public void AddCommand(Command cmd)
    {
        CurrentCommand = cmd;
        gotNewCommand = true;
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
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
    protected override void Update()
    {
        // 本帧的状态
        base.Update();
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

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (stateMachine.CurrentState == airFreeState)
        {
            var velocity = rb.velocity;
            velocity.y -= gravity * Time.deltaTime;
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

}
