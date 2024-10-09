
using System.Collections.Generic;
using UnityEngine;

public abstract class DeathBringerState : IStateMachineState
{
    protected DeathBringer entity;
    protected StateMachine<DeathBringerState> stateMachine;

    public DeathBringerState(DeathBringer deathBringer, StateMachine<DeathBringerState> machine)
    {
        entity = deathBringer;
        stateMachine = machine;
    }

    public virtual void Enter() {}
    public virtual bool Transition()
    {
        if (entity.isDead && this != entity.deathState)
        {
            stateMachine.ChangeState(entity.deathState);
            return true;
        }
        return false;
    }
    public abstract void Update();
    public virtual void Exit() {}
}

public class DeathBringerIdle : DeathBringerState
{
    public DeathBringerIdle(DeathBringer deathBringer, StateMachine<DeathBringerState> machine) : base(deathBringer, machine)
    {
    }

    public override void Enter()
    {
        entity.animator.Play(DeathBringer.ANIM_IDLE);
    }

    public override bool Transition()
    {
        if (base.Transition())
        {
            return true;
        }
        if (entity.CurrentCommand.attack && entity.attackCoolDownTimer <= 0)
        {
            stateMachine.ChangeState(entity.attackState);
            return true;
        }
        if (entity.CurrentCommand.moveX != 0)
        {
            stateMachine.ChangeState(entity.walkState);
            return true;
        }
        return false;
    }

    public override void Update()
    {
        entity.velocity.x = 0;
    }

}

public class DeathBringerWalk : DeathBringerState
{
    public DeathBringerWalk(DeathBringer deathBringer, StateMachine<DeathBringerState> machine) : base(deathBringer, machine)
    {
    }

    public override void Enter()
    {
        entity.animator.Play(DeathBringer.ANIM_WALK);
    }

    public override bool Transition()
    {
        if (base.Transition())
        {
            return true;
        }
        if (entity.CurrentCommand.attack && entity.attackCoolDownTimer <= 0)
        {
            stateMachine.ChangeState(entity.attackState);
            return true;
        }
        if (entity.CurrentCommand.moveX == 0)
        {
            stateMachine.ChangeState(entity.idleState);
            return true;
        }
        return false;
    }

    public override void Update()
    {
        entity.HorizontalMoveAndFlip(entity.CurrentCommand.moveX);
    }
}

public class DeathBringerAttack : DeathBringerState
{
    public DeathBringerAttack(DeathBringer deathBringer, StateMachine<DeathBringerState> machine) : base(deathBringer, machine)
    {
    }

    public override void Enter()
    {
        entity.attackEnd = false;
        entity.velocity.x = 0;
        entity.velocity.y = 0;
        entity.animator.Play(DeathBringer.ANIM_ATTACK);
        entity.hitBox.genHitInfo = GenHitInfo;
    }

    public override bool Transition()
    {
        if (base.Transition())
        {
            return true;
        }
        if (entity.attackEnd)
        {
            stateMachine.ChangeState(entity.idleState);
            return true;
        }
        return false;
    }

    public override void Update()
    {
        if (entity.hitBoxEnabled)
        {
            entity.hitBox.DoHit();
        }
    }

    public override void Exit()
    {
        entity.hitBoxEnabled = false;
        entity.attackCoolDownTimer = entity.attackCoolDownTime;
    }

    private HitInfo GenHitInfo() => new()
    {
        attacker = entity,
        damage = 1,
        force = new Vector2(entity.FacingDirection, 0),
        hitBox = entity.hitBox.hitCollider,
        hitType = HitType.EntityAttack,
    };
}

public class DeathBringerStun : DeathBringerState
{
    private float timer;
    public DeathBringerStun(DeathBringer deathBringer, StateMachine<DeathBringerState> machine) : base(deathBringer, machine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        timer = entity.stunTime;
        entity.animator.Play(DeathBringer.ANIM_IDLE);
    }

    public override bool Transition()
    {
        if (base.Transition())
        {
            return true;
        }
        if (IsEnd)
        {
            stateMachine.ChangeState(entity.idleState);
        }
        return IsEnd;
    }

    public override void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
    }

    private bool IsEnd => timer <= 0;
}

public class DeathBringerDeath : DeathBringerState
{
    public DeathBringerDeath(DeathBringer deathBringer, StateMachine<DeathBringerState> machine) : base(deathBringer, machine)
    {
    }

    public override void Enter()
    {
        entity.animator.Play(DeathBringer.ANIM_DEATH);
        entity.velocity.x = 0;
        entity.velocity.y = 0;
    }

    public override bool Transition()
    {
        return false;
    }

    public override void Update() 
    {
        if (entity.deathEnd)
        {
            GameObject.Destroy(entity.gameObject);
        }
    }
}