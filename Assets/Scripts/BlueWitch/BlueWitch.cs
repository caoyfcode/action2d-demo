using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueWitch : Entity
{
    // TODO 将状态切换条件写为属性, xxxCondition
    // TODO 添加受击强制移动、顿帧、屏幕振动、受击无敌
    // 强制移动可以放到 Entity 中写一个 foreMove 向量, 利用摩擦来减小它
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

    public SummonAttack summonAttack;
    public GameObject smokeRingPrefab;

    #region Command
    public class Command
    {
        public float moveX = 0;
        public bool holdJump = false, startJump = false;
        public bool attack = false;
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
    internal bool AttackCondition => CurrentCommand.attack && airAttackCount < 2;
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
    }
}
