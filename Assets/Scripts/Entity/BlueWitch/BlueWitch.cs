using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BlueWitch : Entity
{
    // TODO 添加顿帧、屏幕振动、受击无敌
    #region Constant Number
    internal float jumpSpeed = 12f;
    internal float airJumpSpeed = 12f;
    internal float maxJumpTime = 0.3f;
    internal float minKeepDirectionTime = 0.15f; // 爬墙跳后 keepDirection, 直到到达最小保持时间且输入反向或跳跃结束
    internal float dashTime = 0.2f;
    internal float dashSpeed = 18f;
    internal float dashCoolDownTime = 0.5f; // 0.2+0.3 相邻两次 dash 的冷却时间
    internal float wallSlideSpeed = 2f;
    internal float attackComboTime = 0.3f;
    internal int maxAirAttackCount = 2; // 依据召唤的替身攻击类型设置空中攻击次数
    internal float hitInvincibleTime = 1f;
    internal const string ANIM_IDLE = "Idle";
    internal const string ANIM_RUN = "Run";
    internal const string ANIM_ATTACK = "Attack";
    internal const string ANIM_CHARGE = "Charge";
    #endregion

    public SummonAttack summonAttack;
    public SummonProjectile summonProjectile;
    public SpawnAfterImage spawnAfterImage;
    public GameObject wallSlideCrow;
    public GameObject smokeRingPrefab;

    #region Command

    public class Command
    {
        public float moveX = 0;
        public bool holdJump = false, startJump = false;
        public bool dash = false;
        public bool attackProjectileBat = false;
        public bool attackHeroKnight = false;
        public AttackType AttackType
        {
            get
            {
                if (attackHeroKnight)
                    return AttackType.HeroKnight;
                if (attackProjectileBat)
                    return AttackType.ProjectileBat;
                return AttackType.None;
            }
        }
    }
    internal Command CurrentCommand { get; private set; }
    #endregion

    #region Attack Info
    public enum AttackType
    {
        None,
        HeroKnight,
        ProjectileBat,
    }
    internal AttackType attackBuffer = AttackType.None;
    #endregion

    #region Status
    internal int airJumpCount = 0;
    internal int airAttackCount = 0;
    internal int airDashCount = 0;
    internal float dashCoolDownTimer = 0;
    internal float hitInvincibleTimer = 0;
    internal bool stateControlInvincible = false;
    public BlueWitchStatus status;
    public void ResetAirAttackCount() => airAttackCount = 0; // 暂时只有一个
    public void AddAirAttackCount(AttackType type)
    {
        switch (type)
        {
            case AttackType.HeroKnight:
                airAttackCount++;
                break;
        }
    }
    public bool TestAttackCount(AttackType type)
    {
        switch (type)
        {
            case AttackType.HeroKnight:
                return airAttackCount < maxAirAttackCount;
            case AttackType.ProjectileBat:
                return true;
            default: // no attack command
                return false;
        }
    }
    public override bool IsInvincible => hitInvincibleTimer > 0 || stateControlInvincible;
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
    internal bool DashCondition => status.dashAbility && CurrentCommand.dash && dashCoolDownTimer <= 0 && airDashCount == 0;
    internal AttackType CurrentAttack
    {
        get
        {
            if (attackBuffer != AttackType.None && TestAttackCount(attackBuffer))
            {
                return attackBuffer;
            }
            if (TestAttackCount(CurrentCommand.AttackType))
            {
                return CurrentCommand.AttackType;
            }
            return AttackType.None;
        }
    }
    internal bool AttackCondition => CurrentAttack != AttackType.None;
    internal bool JumpCondition => CurrentCommand.startJump && !isOnCeiling;
    internal bool AirJumpCondition => status.secondJumpAbility && airJumpCount <= 0 && JumpCondition;
    internal bool WallSlideCondition => status.wallSlideAbility && !isOnGround && IsFacingWall;
    #endregion

    public void SetCommand(Command cmd)
    {
        CurrentCommand = cmd;
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
        wallSlideCrow.SetActive(false);
    }

    // Update is called once per frame
    protected override void Update()
    {
        // 本帧的状态
        base.Update();
        velocity = rb.velocity;
        UpdateTimers();
        stateMachine.Update();
        MergeVelocity();
        rb.velocity = velocity;
    }

    protected virtual void FixedUpdate()
    {
        if (stateMachine.CurrentState == airFreeState)
        {
            var velocity = rb.velocity;
            if (velocity.y > Mathf.Epsilon && velocity.y <= airJumpSpeed) // 向上跳跃时, 希望尽可能以高速达到高度
            {
                if (velocity.y > airJumpSpeed / 2)
                {
                    velocity.y -= 0.5f * gravity * Time.fixedDeltaTime;
                }
                else // 向上低速的时间尽量少
                {
                    velocity.y -= 2 * gravity * Time.fixedDeltaTime;
                }
            }
            else
            {
                velocity.y -= gravity * Time.fixedDeltaTime;
            }
            velocity.y -= gravity * Time.fixedDeltaTime;
            rb.velocity = velocity;
        }
    }

    void UpdateTimers()
    {
        if (dashCoolDownTimer > 0)
            dashCoolDownTimer -= Time.deltaTime;
        if (hitInvincibleTimer > 0)
            hitInvincibleTimer -= Time.deltaTime;
    }

    public void ChangeInputDirectionAndFlip(float direction)
    {
        CurrentCommand.moveX = direction;
        HorizontalMoveAndFlip(direction);
    }

    public override void OnAttacked(HitInfo hitInfo)
    {
        base.OnAttacked(hitInfo);
        status.heath -= hitInfo.damage;
        hitInvincibleTimer = hitInvincibleTime;
        // TODO 死亡后重新加载, 与主动坐火刷新同时实现
    }
}
