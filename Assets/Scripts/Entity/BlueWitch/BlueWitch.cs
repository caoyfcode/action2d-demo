using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BlueWitch : Entity
{
    #region Constant Number
    internal const string ANIM_IDLE = "Idle";
    internal const string ANIM_RUN = "Run";
    internal const string ANIM_ATTACK = "Attack";
    internal const string ANIM_CHARGE = "Charge";
    #endregion

    public SummonAttack summonAttack;
    public SummonProjectile summonProjectile;
    public SummonDefend summonDenfend;
    public SpawnAfterImage spawnAfterImage;
    public GameObject wallSlideCrow;

    #region Command
    public class Command
    {
        public float speedX = 0;
        public bool holdJump = false, startJump = false;
        public bool dash = false;
        public bool attackArcaneArcher = false;
        public bool attackHeroKnight = false;
        public bool defendFireKnight = false;
        public AttackType AttackType
        {
            get
            {
                if (attackHeroKnight)
                    return AttackType.HeroKnight;
                if (attackArcaneArcher)
                    return AttackType.ArcaneArcher;
                if (defendFireKnight)
                    return AttackType.DefendFireKnight;
                return AttackType.None;
            }
        }
        public AttackType TakeAttackType()
        {
            var ret = AttackType;
            attackArcaneArcher = false;
            attackHeroKnight = false;
            defendFireKnight = false;
            return ret;
        }

        public void SpeedX(float speedX) => this.speedX = speedX;
        public void Jump(bool startJump, bool holdJump) => (this.startJump, this.holdJump) = (startJump, holdJump);
        public void Dash(bool dash=true) => this.dash = dash;
        public void Attack(AttackType attackType)
        {
            switch (attackType)
            {
                case AttackType.ArcaneArcher:
                    attackArcaneArcher = true;
                    break;
                case AttackType.HeroKnight:
                    attackHeroKnight = true;
                    break;
                case AttackType.DefendFireKnight:
                    defendFireKnight = true;
                    break;
            }
        }
    }
    public Command CurrentCommand { get; private set; }
    #endregion

    #region Attack Info
    public enum AttackType
    {
        None,
        HeroKnight,
        ArcaneArcher,
        DefendFireKnight,
    }
    internal AttackType attackBuffer = AttackType.None;
    #endregion

    #region Status
    internal int airJumpCount = 0;
    internal int airAttackCount = 0;
    internal int airDashCount = 0;
    internal float dashCoolDownTimer = 0;
    internal float hitInvincibleTimer = 0;
    internal float hitStunTimer = 0; // hitstun 用来表示受击后无法控制移动的状态，此时水平由 forceMove 控制, 竖直方向受到重力影响
    internal bool stateControlInvincible = false;
    public BlueWitchInfo info;
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
                return airAttackCount < info.maxAirAttackCount;
            case AttackType.ArcaneArcher:
                return true;
            case AttackType.DefendFireKnight:
                return true;
            default: // no attack command
                return false;
        }
    }
    public override bool IsInvincible => base.IsInvincible || hitInvincibleTimer > 0 || stateControlInvincible;
    public bool IsHitStun => hitStunTimer > 0 || forceMove != 0;
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
    internal bool DashCondition => info.dashAbility && CurrentCommand.dash && dashCoolDownTimer <= 0 && airDashCount == 0;
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
    internal bool AirJumpCondition => info.secondJumpAbility && airJumpCount <= 0 && JumpCondition;
    internal bool WallSlideCondition => info.wallSlideAbility && !isOnGround && IsFacingWall;
    #endregion

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
        CurrentCommand = new Command();
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
            if (velocity.y > Mathf.Epsilon && velocity.y <= info.airJumpSpeed && !IsHitStun) // 向上跳跃时, 希望尽可能以高速达到高度
            {
                if (velocity.y > info.airJumpSpeed / 2)
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
            // velocity.y -= gravity * Time.fixedDeltaTime;
            rb.velocity = velocity;
        }
    }

    void UpdateTimers()
    {
        if (dashCoolDownTimer > 0)
            dashCoolDownTimer -= Time.deltaTime;
        if (hitInvincibleTimer > 0)
            hitInvincibleTimer -= Time.deltaTime;
        if (hitStunTimer > 0)
            hitStunTimer -= Time.deltaTime;
    }

    public void ChangeInputDirection(float direction)
    {
        CurrentCommand.speedX = direction * Math.Abs(CurrentCommand.speedX);
        velocity.x = CurrentCommand.speedX;
    }

    public override void HitSuccess(HitInfo hitInfo)
    {
        base.HitSuccess(hitInfo);
        if (hitInfo.hitType == HitType.EntityAttack && stateMachine.CurrentState == attackState)
        {
            attackState.OnSuccess(hitInfo);
            WorldManager.CameraController.StartShake(hitInfo.force, 0.1f, 10);
        }
    }

    public override void TakeHit(HitInfo hitInfo)
    {
        base.TakeHit(hitInfo);
        WorldManager.CameraController.StartShake(hitInfo.force, 0.2f, 10);
        info.health -= hitInfo.damage;
        hitInvincibleTimer = info.hitInvincibleTime;
        hitStunTimer = info.hitStunTime;
        AddForce(hitInfo.force);
        // TODO 死亡后重新加载, 与主动坐火刷新同时实现
    }

    public void Heal()
    {
        info.health = info.maxHealth;
        info.magic = info.maxMagic;
        airJumpCount = 0;
        airAttackCount = 0;
        airDashCount = 0;
        dashCoolDownTimer = 0;
        stateControlInvincible = true;
    }
}
