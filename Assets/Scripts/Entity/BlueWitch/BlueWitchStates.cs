using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BlueWitchState : IStateMachineState
{
    protected BlueWitch entity;
    protected StateMachine<BlueWitchState> stateMachine;

    public BlueWitchState(BlueWitch entity, StateMachine<BlueWitchState> machine)
    {
        this.entity = entity;
        stateMachine = machine;
    }

    public virtual void Enter() { }
    public abstract bool Transition();
    public abstract void Update();
    public virtual void Exit() { }
}


public class BlueWitchGroundFree : BlueWitchState
{
    public BlueWitchGroundFree(BlueWitch entity, StateMachine<BlueWitchState> machine) : base(entity, machine)
    {
    }

    public override void Enter()
    {
        entity.airJumpCount = 0;
        entity.ResetAirAttackCount();
        entity.airDashCount = 0;
    }

    public override bool Transition()
    {
        bool changed = true;
        var cmd = entity.CurrentCommand;
        if (entity.DashCondition)
        {
            stateMachine.ChangeState(entity.dashState);
        }
        else if (entity.AttackCondition)
        {
            stateMachine.ChangeState(entity.attackState);
        }
        else if (entity.JumpCondition)
        {
            stateMachine.ChangeState(entity.holdJumpState);
            if (entity.IsFacingWall)
                entity.holdJumpState.jumpFromWallFoot = true;
        }
        else if (!entity.isOnGround)
        {
            stateMachine.ChangeState(entity.airFreeState);
        }
        else
        {
            changed = false;
        }
        return changed;
    }

    public override void Update()
    {
        if (entity.CurrentCommand.moveX != 0)
        {
            entity.animator.Play(BlueWitch.ANIM_RUN);
        }
        else
        {
            entity.animator.Play(BlueWitch.ANIM_IDLE);
        }

        // entity.velocity.y = 0;
        entity.HorizontalMoveAndFlip(entity.CurrentCommand.moveX);
    }
}

public abstract class BlueWitchAirState : BlueWitchState
{
    // 若从墙脚起跳，则不会转到爬墙状态
    public bool jumpFromWallFoot = false;
    // 爬墙跳后 keepDirection, 直到到达最小保持时间且输入反向或跳跃结束
    protected float keepDirectionTimer = 0f;
    protected float keepDirection = 1f;
    protected bool shouldKeepDirection = false;

    protected float MoveX
    {
        get
        {
            if (!shouldKeepDirection || (keepDirectionTimer <= 0 && entity.CurrentCommand.moveX * keepDirection < 0))
            { // 不需要 keep direction 或者超出 minKeepDirectionTime 且反向输入
                return entity.CurrentCommand.moveX;
            }

            return keepDirection;
        }
    }
    public BlueWitchAirState(BlueWitch entity, StateMachine<BlueWitchState> machine) : base(entity, machine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        keepDirectionTimer = 0;
        shouldKeepDirection = false;
    }

    // 在 WallSlideState 调用
    public void KeepDirection()
    {
        shouldKeepDirection = true;
        keepDirectionTimer = entity.minKeepDirectionTime;
        keepDirection = entity.FacingDirection;
        entity.HorizontalMoveAndFlip(MoveX);
    }

    public override bool Transition()
    {
        bool changed = true;
        if (entity.DashCondition)
        {
            stateMachine.ChangeState(entity.dashState);
        }
        else if (entity.AttackCondition)
        {
            stateMachine.ChangeState(entity.attackState);
        }
        else if (entity.WallSlideCondition && MoveX * entity.FacingDirection > 0 && !jumpFromWallFoot)
        {
            stateMachine.ChangeState(entity.wallSlideState);
        }
        else if (entity.isOnCeiling && entity.velocity.y > 0) // 碰到头顶
        {
            if (this == entity.airFreeState)
                changed = false;
            entity.velocity.y = 0;
            stateMachine.ChangeState(entity.airFreeState);
        }
        else
        {
            changed = false;
        }
        return changed;
    }

    public override void Update()
    {
        if (keepDirectionTimer > 0)
        {
            keepDirectionTimer -= Time.deltaTime;
        }
        else
        {
            keepDirectionTimer = 0;
        }
        if (MoveX != 0)
        {
            entity.animator.Play(BlueWitch.ANIM_RUN);
        }
        else
        {
            entity.animator.Play(BlueWitch.ANIM_IDLE);
        }
        entity.HorizontalMoveAndFlip(MoveX);
    }

    public override void Exit()
    {
        base.Exit();
        jumpFromWallFoot = false;
        keepDirectionTimer = 0f;
    }
}

/// airFree 状态同时作为所有状态结束时到达的状态
/// 至于为什么不是groundFree, 因为进入地面状态会清除一些计数器
public class BlueWitchAirFree : BlueWitchAirState
{
    private float coyoteTimer = 0f;
    public BlueWitchAirFree(BlueWitch entity, StateMachine<BlueWitchState> machine) : base(entity, machine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        if (stateMachine.PrevState == entity.groundFreeState || stateMachine.PrevState == entity.wallSlideState)
        {
            coyoteTimer = 0.1f;
        }
        else
        {
            coyoteTimer = 0f;
        }
    }

    public override bool Transition()
    {
        coyoteTimer -= Time.deltaTime;
        // 这里先判断地是因为该有的状态转换地上都有
        // 并且进入地上可以重置一些计数器
        if (entity.isOnGround)
        {
            stateMachine.ChangeState(entity.groundFreeState);
            return true;
        }
        if (base.Transition())
            return true;
        if (entity.JumpCondition && coyoteTimer > 0)
        {
            stateMachine.ChangeState(entity.holdJumpState);
            if (entity.stateMachine.PrevState == entity.wallSlideState)
            {
                entity.holdJumpState.KeepDirection();
            }
            return true;
        }
        if (entity.AirJumpCondition)
        {
            entity.velocity.y = entity.airJumpSpeed;
            entity.airJumpCount++;
            entity.ResetAirAttackCount(); // 跳跃重置攻击
            UnityEngine.Object.Instantiate(Game.VFXLoader.smokeRingPrefab, new Vector3(entity.transform.position.x, entity.transform.position.y - 0.59f, 0), Quaternion.identity);
        }
        return false;
    }
}


public class BlueWitchHoldJump : BlueWitchAirState
{
    float timer = 0;
    public BlueWitchHoldJump(BlueWitch entity, StateMachine<BlueWitchState> machine) : base(entity, machine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        timer = entity.maxJumpTime;
        //entity.animator.Play(BlueWitch.ANIM_IDLE);
        entity.velocity.y = entity.jumpSpeed;
        entity.CurrentCommand.startJump = false; // 消耗掉 start jump
    }

    public override bool Transition()
    {
        if (base.Transition())
            return true;
        if (IsEnd)
        {
            stateMachine.ChangeState(entity.airFreeState);
            return true;
        }
        return false;
    }

    public override void Update()
    {
        base.Update();
        timer -= Time.deltaTime;
        entity.velocity.y = entity.jumpSpeed;
    }

    public override void Exit()
    {
        base.Exit();
        timer = 0;
        entity.velocity.y = 0;
    }

    bool IsEnd => timer <= 0 || !entity.CurrentCommand.holdJump;
}

public class BlueWitchDash : BlueWitchState
{
    float timer = 0;
    public BlueWitchDash(BlueWitch entity, StateMachine<BlueWitchState> machine) : base(entity, machine)
    {
    }

    public override void Enter()
    {
        timer = entity.dashTime;
        entity.dashCoolDownTimer = entity.dashCoolDownTime;
        //entity.animator.Play(BlueWitch.ANIM_ATTACK, -1, 0);
        entity.animator.Play(BlueWitch.ANIM_RUN);
        entity.velocity.y = 0;
        entity.velocity.x = entity.dashSpeed * entity.FacingDirection;
        if (!entity.isOnGround)
        {
            entity.airDashCount++;
        }

        entity.stateControlInvincible = true;
        entity.spawnAfterImage.Spawn = true;
    }

    public override bool Transition()
    {
        if (entity.WallSlideCondition)
        {
            stateMachine.ChangeState(entity.wallSlideState);
            return true;
        }
        if (IsEnd)
        {
            stateMachine.ChangeState(entity.airFreeState);
            return true;
        }
        return false;
    }

    public override void Update()
    {
        timer -= Time.deltaTime;
        entity.velocity.y = 0;
        entity.velocity.x = entity.dashSpeed * entity.FacingDirection;
        // TODO 拖尾
    }

    public override void Exit()
    {
        timer = 0;
        entity.stateControlInvincible = false;
        entity.spawnAfterImage.Spawn = false;
    }

    bool IsEnd => timer <= 0;
}

public class BlueWitchAttack : BlueWitchState
{
    private Summon currentSummon;
    private BlueWitch.AttackType currentType;
    private float stopTimer = 0;
    public BlueWitchAttack(BlueWitch entity, StateMachine<BlueWitchState> machine) : base(entity, machine)
    {
    }

    public override void Enter()
    {
        currentType = entity.CurrentAttack;
        currentSummon = currentType switch
        {
            BlueWitch.AttackType.HeroKnight => entity.summonAttack,
            BlueWitch.AttackType.ArcaneArcher => entity.summonProjectile,
            BlueWitch.AttackType.DefendFireKnight => entity.summonDenfend,
            _ => entity.summonAttack
        };
        entity.attackBuffer = BlueWitch.AttackType.None;
        currentSummon.StartSummon();
        entity.velocity.x = 0;
        entity.velocity.y = 0;
        entity.animator.Play(BlueWitch.ANIM_ATTACK, -1, 0);
        if (!entity.isOnGround)
        {
            entity.AddAirAttackCount(currentType);
        }
        stopTimer = 0f;
        if (currentType == BlueWitch.AttackType.DefendFireKnight)
        {
            entity.stateControlInvincible = true;
        }
        entity.CurrentCommand.TakeAttackType(); // 消耗掉输入的攻击指令
    }

    public override bool Transition()
    {
        if (currentSummon.isCancelable && entity.DashCondition)
        {
            // airFree 也可进入 dash, 并且还能处理进入地面的情况
            stateMachine.ChangeState(entity.airFreeState);
            currentSummon.comboTimer = 0;
            return true;
        }
        if (currentSummon.isEnd)
        {
            if (entity.AttackCondition)
            {
                stateMachine.ChangeState(entity.attackState);
            }
            else
            {
                stateMachine.ChangeState(entity.airFreeState);
            }
            return true;
        }
        return false;
    }

    public override void Update()
    {
        if (stopTimer > 0)
        {
            stopTimer -= Time.unscaledDeltaTime;
            entity.animator.speed = 0.5f;
            entity.summonAttack.animator.speed = 0.5f;
        }
        else
        {
            entity.animator.speed = 1f;
            entity.summonAttack.animator.speed = 1;
        }
        // TODO 改为 shouldBuffer
        if (!currentSummon.isCancelable && entity.CurrentCommand.AttackType != BlueWitch.AttackType.None)
        {
            entity.attackBuffer = entity.CurrentCommand.AttackType;
        }
    }

    public override void Exit()
    {
        currentSummon.OnSummonEnd();
        entity.animator.speed = 1f;
        entity.summonAttack.animator.speed = 1;
        entity.stateControlInvincible = false;
    }

    public void OnSuccess(HitInfo hitInfo)
    {
        stopTimer = 0.2f;
    }
}

public class BlueWitchWallSlide : BlueWitchState
{
    public BlueWitchWallSlide(BlueWitch entity, StateMachine<BlueWitchState> machine) : base(entity, machine)
    {
    }

    public override void Enter()
    {
        entity.airJumpCount = 0;
        entity.ResetAirAttackCount();
        entity.airDashCount = 0;
        entity.velocity.x = 0;
        entity.velocity.y = -entity.wallSlideSpeed;
        entity.animator.Play(BlueWitch.ANIM_RUN);
        entity.wallSlideCrow.SetActive(true);
    }

    public override bool Transition()
    {
        if (entity.DashCondition)
        {
            entity.ChangeInputDirectionAndFlip(-entity.FacingDirection);
            stateMachine.ChangeState(entity.dashState);
            return true;
        }
        if (entity.JumpCondition)
        {
            entity.ChangeInputDirectionAndFlip(-entity.FacingDirection);
            stateMachine.ChangeState(entity.holdJumpState);
            entity.holdJumpState.KeepDirection();
            return true;
        }
        if (entity.isOnGround || !entity.IsFacingWall || entity.CurrentCommand.moveX * entity.FacingDirection < 0)
        {
            stateMachine.ChangeState(entity.airFreeState);
            return true;
        }
        return false;
    }

    public override void Update()
    {
        entity.velocity.x = 0;
        entity.velocity.y = -entity.wallSlideSpeed;
    }

    public override void Exit()
    {
        base.Exit();
        entity.wallSlideCrow.SetActive(false);
    }
}