using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueWitch : Entity
{
    // TODO 添加顿帧、屏幕振动、受击无敌
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
    internal const string ANIM_ATTACK = "Attack";
    internal const string ANIM_CHARGE = "Charge";
    #endregion

    // TODO GameObject 下将有多个 SummonAttack 结点, 要选择其中之一使用, 选择时也要设置 maxAirAttackCount
    public SummonAttack summonAttack;
    public SummonProjectile summonProjectile;
    public GameObject smokeRingPrefab;

    #region Command
    public class Command
    {
        public float moveX = 0;
        public bool holdJump = false, startJump = false;
        public bool attack = false;
        public bool summon = false;
        public bool dash = false;
        public bool slideDown = false;
    }
    internal Command CurrentCommand { get; private set; }
    private bool gotNewCommand = false;
    #endregion

    #region Abilitiy
    [Header("Ability")]
    public bool secondJumpAbility = true;
    public bool dashAbility = true;
    public bool wallSlideAbility = true;
    #endregion

    #region Status
    internal int airJumpCount = 0;
    internal int airAttackCount = 0;
    internal int maxAirAttackCount = 2; // 依据召唤的替身攻击类型设置空中攻击次数
    internal int airDashCount = 0;
    internal float dashCoolDownTimer = 0;
    #endregion

    #region StateMachine
    internal StateMachine<BlueWitchState> stateMachine;
    internal BlueWitchGroundFree groundFreeState;
    internal BlueWitchAirFree airFreeState;
    internal BlueWitchHoldJump holdJumpState;
    internal BlueWitchAttack attackState;
    internal BlueWitchDash dashState;
    internal BlueWitchWallSlide wallSlideState;
    // 状态转换条件中的通用部分
    internal bool DashCondition => dashAbility && CurrentCommand.dash && dashCoolDownTimer <= 0 && airDashCount == 0;
    internal bool AttackCondition => CurrentCommand.attack && airAttackCount < maxAirAttackCount;
    internal bool JumpCondition => CurrentCommand.startJump && !isOnCeiling;
    internal bool AirJumpCondition => secondJumpAbility && airJumpCount <= 0 && JumpCondition;
    internal bool WallSlideCondition => wallSlideAbility && !isOnGround && IsFacingWall;
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
        stateMachine = new StateMachine<BlueWitchState>();
        groundFreeState = new BlueWitchGroundFree(this, stateMachine);
        airFreeState = new BlueWitchAirFree(this, stateMachine);
        holdJumpState = new BlueWitchHoldJump(this, stateMachine);
        attackState = new BlueWitchAttack(this, stateMachine);
        dashState = new BlueWitchDash(this, stateMachine);
        wallSlideState = new BlueWitchWallSlide(this, stateMachine);
        stateMachine.Init(airFreeState);
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
        if (CurrentCommand.summon)
        {
            summonProjectile.StartSummon();
        }
        gotNewCommand = false;
        MergeVelocity();
        rb.velocity = velocity;
    }

    protected virtual void FixedUpdate()
    {
        if (stateMachine.CurrentState == airFreeState)
        {
            var velocity = rb.velocity;
            velocity.y -= gravity * Time.fixedDeltaTime;
            rb.velocity = velocity;
        }
    }

    void UpdateTimers()
    {
        if (dashCoolDownTimer > 0)
            dashCoolDownTimer -= Time.deltaTime;
    }

    public void ChangeInputDirectionAndFlip(float direction)
    {
        CurrentCommand.moveX = direction;
        HorizontalMoveAndFlip(direction);
    }
}
