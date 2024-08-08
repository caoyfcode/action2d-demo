using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WarriorState : IStateMachineState
{
    protected Warrior warrior;
    protected StateMachine<WarriorState> stateMachine;

    public WarriorState(Warrior warrior, StateMachine<WarriorState> machine)
    {
        this.warrior = warrior;
        this.stateMachine = machine;
    }

    public virtual void Enter() {}
    public abstract bool Transition();
    public abstract void Update();
    public virtual void Exit() {}
}


public class WarriorGroundFree : WarriorState
{
    public WarriorGroundFree(Warrior warrior, StateMachine<WarriorState> machine) : base(warrior, machine)
    {
    }

    public override void Enter()
    {
        warrior.airJumpCount = 0;
        warrior.airAttackCount = 0;
        warrior.airDashCount = 0;
    }

    public override bool Transition()
    {
        bool changed = true;
        var cmd = warrior.CurrentCommand;
        if (cmd.dash && warrior.dashCoolDownTimer <= 0 && warrior.airDashCount == 0 && warrior.dashAbility)
        {
            stateMachine.ChangeState(warrior.dashState);
        }
        else if (cmd.attack && warrior.airAttackCount < 2)
        {
            stateMachine.ChangeState(warrior.attackState);
        }
        else if (cmd.startJump && !warrior.isOnCeiling)
        {
            stateMachine.ChangeState(warrior.holdJumpState);
            if (warrior.IsFacingWall)
                warrior.holdJumpState.jumpFromWallFoot = true;
        }
        else if (!warrior.isOnGround)
        {
            stateMachine.ChangeState(warrior.airFreeState);
        }
        else
        {
            changed = false;
        }
        return changed;
    }

    public override void Update()
    {
        if (warrior.CurrentCommand.moveX != 0)
        {
            warrior.animator.Play(Warrior.ANIM_RUN);
        }
        else
        {
            warrior.animator.Play(Warrior.ANIM_IDLE);
        }

        warrior.velocity.y = 0;
        warrior.HorizontalMoveAndFlip(warrior.CurrentCommand.moveX);
    }
}

public abstract class WarriorAirState : WarriorState
{
    // 若从墙脚起跳，则不会转到爬墙状态
    public bool jumpFromWallFoot = false;
    public WarriorAirState(Warrior warrior, StateMachine<WarriorState> machine) : base(warrior, machine)
    {
    }

    public override bool Transition()
    {
        bool changed = true;
        var cmd = warrior.CurrentCommand;
        if (cmd.dash && warrior.dashCoolDownTimer <= 0 && warrior.airDashCount == 0 && warrior.dashAbility)
        {
            stateMachine.ChangeState(warrior.dashState);
        }
        else if (cmd.attack && warrior.airAttackCount < 2)
        {
            stateMachine.ChangeState(warrior.attackState);
        }
        else if (warrior.IsFacingWall && cmd.moveX * warrior.FacingDirection > 0 && !jumpFromWallFoot && warrior.wallSlideAbility)
        {
            stateMachine.ChangeState(warrior.wallSlideState);
        }
        else if (warrior.isOnCeiling && warrior.velocity.y > 0) // 碰到头顶
        {
            if (this == warrior.airFreeState)
                changed = false;
            warrior.velocity.y = 0;
            stateMachine.ChangeState(warrior.airFreeState);
        }
        else
        {
            changed = false;
        }
        return changed;
    }

    public override void Update()
    {
        if (warrior.velocity.y > 0)
        {
            warrior.animator.Play(Warrior.ANIM_RISE);
        }
        else
        {
            warrior.animator.Play(Warrior.ANIM_FALL);
        }
        warrior.HorizontalMoveAndFlip(warrior.CurrentCommand.moveX);
    }

    public override void Exit()
    {
        base.Exit();
        jumpFromWallFoot = false;
    }
}

/// airFree 状态同时作为所有状态结束时到达的状态
/// 至于为什么不是groundFree, 因为进入地面状态会清除一些计数器
public class WarriorAirFree : WarriorAirState
{
    public WarriorAirFree(Warrior warrior, StateMachine<WarriorState> machine) : base(warrior, machine)
    {
    }

    public override bool Transition()
    {
        // 这里先判断地是因为该有的状态转换地上都有
        // 并且进入地上可以重置一些计数器
        if (warrior.isOnGround)
        {
            stateMachine.ChangeState(warrior.groundFreeState);
            return true;
        }
        if (base.Transition())
            return true;
        if (warrior.CurrentCommand.startJump && warrior.airJumpCount == 0 && warrior.secondJumpAbility) // air jump
        {
            warrior.velocity.y = warrior.airJumpSpeed;
            warrior.airJumpCount++;
            warrior.airAttackCount = 0; // 跳跃重置攻击
            UnityEngine.Object.Instantiate(warrior.smokeRingPrefab, new Vector3(warrior.transform.position.x - 0.1f, warrior.transform.position.y - 0.7f, 0), Quaternion.identity);
        }
        return false;
    }

    public override void Update()
    {
        base.Update();
    }
}


public class WarriorHoldJump : WarriorAirState
{
    float timer = 0;
    public WarriorHoldJump(Warrior warrior, StateMachine<WarriorState> machine) : base(warrior, machine)
    {
    }

    public override void Enter()
    {
        timer = warrior.maxJumpTime;
        warrior.animator.Play(Warrior.ANIM_RISE);
        warrior.velocity.y = warrior.jumpSpeed;
        warrior.HorizontalMoveAndFlip(warrior.CurrentCommand.moveX);
        warrior.CurrentCommand.startJump = false; // 消耗掉 start jump
    }

    public override bool Transition()
    {
        if (base.Transition())
            return true;
        if (IsEnd)
        { 
            stateMachine.ChangeState(warrior.airFreeState);
            return true;
        }
        return false;
    }

    public override void Update()
    {  
        base.Update();
        timer -= Time.deltaTime;
        warrior.velocity.y = warrior.jumpSpeed;
    }

    public override void Exit()
    {
        base.Exit();
        timer = 0;
    }

    bool IsEnd => timer <= 0 || !warrior.CurrentCommand.holdJump;
}

public class WarriorDash : WarriorState
{
    float timer = 0;
    public WarriorDash(Warrior warrior, StateMachine<WarriorState> machine) : base(warrior, machine)
    {
    }

    public override void Enter()
    {
        timer = warrior.dashTime;
        warrior.dashCoolDownTimer = warrior.dashCoolDownTime;
        warrior.animator.Play(Warrior.ANIM_DASH);
        // 调用 horizon... 是为了在冲刺前转向
        warrior.HorizontalMoveAndFlip(warrior.CurrentCommand.moveX);
        warrior.velocity.y = 0;
        warrior.velocity.x = warrior.dashSpeed * warrior.FacingDirection;
        if (!warrior.isOnGround)
        {
            warrior.airDashCount++;
        }
    }

    public override bool Transition()
    {
        if (IsEnd)
        {
            if (!warrior.isOnGround && warrior.IsFacingWall && warrior.wallSlideAbility)
                stateMachine.ChangeState(warrior.wallSlideState);
            else
                stateMachine.ChangeState(warrior.airFreeState);
            return true;
        }
        return false;
    }

    public override void Update()
    {
        timer -= Time.deltaTime;
        warrior.velocity.y = 0;
        warrior.velocity.x = warrior.dashSpeed * warrior.FacingDirection;
        // TODO 拖尾
    }

    public override void Exit()
    {
        timer = 0;
    }

    bool IsEnd => timer <= 0;
}

public class WarriorAttack : WarriorState
{
    string nextAnimName = Warrior.ANIM_ATTACK_1;
    bool attackBuffer;
    private ContactFilter2D hitBoxFilter = new();
    private Collider2D[] hitResults = new Collider2D[5];
    private HashSet<Entity> attatckedObjects = new();
    public WarriorAttack(Warrior warrior, StateMachine<WarriorState> machine) : base(warrior, machine)
    {
        hitBoxFilter.SetLayerMask(LayerMask.GetMask("Enemy"));
    }

    public override void Enter()
    {
        attackBuffer = false;
        warrior.attackCancelable = true;
        warrior.attackEnd = false;
        warrior.velocity.x = 0;
        warrior.velocity.y = 0;
        if (warrior.attackComboTimer <= 0)
            nextAnimName = Warrior.ANIM_ATTACK_1;
        warrior.animator.Play(nextAnimName);
        nextAnimName = nextAnimName == Warrior.ANIM_ATTACK_1 ? Warrior.ANIM_ATTACK_2 : Warrior.ANIM_ATTACK_1;
        if (!warrior.isOnGround)
        {
            warrior.airAttackCount++;
        }
        attatckedObjects.Clear();
    }

    public override bool Transition()
    {
        if (warrior.attackCancelable && warrior.CurrentCommand.dash && warrior.dashCoolDownTimer <= 0 && warrior.airDashCount == 0 && warrior.dashAbility)
        {
            // airFree 也可进入 dash, 并且还能处理进入地面的情况
            warrior.attackComboTimer = 0;
            stateMachine.ChangeState(warrior.airFreeState);
            return true;
        }
        if (warrior.attackEnd)
        {
            warrior.attackComboTimer = warrior.attackComboTime;
            if (!attackBuffer)
            {
                stateMachine.ChangeState(warrior.airFreeState);
            }
            else if (warrior.airAttackCount < 2)
            {
                stateMachine.ChangeState(warrior.attackState);
            }
            return true;
        }
        return false;
    }

    public override void Update()
    {
        if (warrior.hitBoxEnabled)
        {
            int hitNum = warrior.hitBox.OverlapCollider(hitBoxFilter, hitResults);
            for (int i = 0; i < hitNum; i++)
            {
                Entity target = hitResults[i].GetComponentInParent<Entity>();
                if (target && !attatckedObjects.Contains(target))
                {
                    target.OnAttacked(new AttackInfo()
                    {
                        attacker = warrior,
                        hitBox = warrior.hitBox,
                        hurtBox = hitResults[i],
                    });
                    attatckedObjects.Add(target);
                }
            }
        }
        if (!warrior.attackCancelable && warrior.CurrentCommand.attack)
        {
            attackBuffer = true;
        }
    }

    public override void Exit()
    {
        warrior.hitBoxEnabled = false;
        attatckedObjects.Clear();
    }

}

public class WarriorWallSlide : WarriorState
{
    public WarriorWallSlide(Warrior warrior, StateMachine<WarriorState> machine) : base(warrior, machine)
    {
    }

    public override void Enter()
    {
        warrior.airJumpCount = 0;
        warrior.airAttackCount = 0;
        warrior.airDashCount = 0;
        warrior.velocity.x = 0;
        warrior.velocity.y = -warrior.wallSlideSpeed;
        warrior.animator.Play(Warrior.ANIM_WALL_SLIDE);
    }

    public override bool Transition()
    {
        if (warrior.CurrentCommand.dash && warrior.dashCoolDownTimer <= 0 && warrior.airDashCount == 0 && warrior.dashAbility && warrior.CurrentCommand.moveX * warrior.FacingDirection < 0)
        {
            stateMachine.ChangeState(warrior.dashState);
            return true;
        }
        if (warrior.CurrentCommand.startJump && !warrior.isOnCeiling && warrior.CurrentCommand.moveX * warrior.FacingDirection < 0)
        {
            stateMachine.ChangeState(warrior.holdJumpState);
            return true;
        }
        if (warrior.isOnGround || !warrior.IsFacingWall || warrior.CurrentCommand.moveX * warrior.FacingDirection < 0)
        {
            stateMachine.ChangeState(warrior.airFreeState);
            return true;
        }
        return false;
    }

    public override void Update()
    {
        warrior.velocity.x = 0;
        warrior.velocity.y = -warrior.wallSlideSpeed;
    }
}