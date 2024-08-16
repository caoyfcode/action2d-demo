
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
    private ContactFilter2D hitBoxFilter = new();
    private Collider2D[] hitResults = new Collider2D[5];
    private HashSet<Entity> attatckedObjects = new HashSet<Entity>();
    public DeathBringerAttack(DeathBringer deathBringer, StateMachine<DeathBringerState> machine) : base(deathBringer, machine)
    {
        hitBoxFilter.SetLayerMask(LayerMask.GetMask("Player"));
    }

    public override void Enter()
    {
        entity.attackEnd = false;
        entity.velocity.x = 0;
        entity.velocity.y = 0;
        entity.animator.Play(DeathBringer.ANIM_ATTACK);
        attatckedObjects.Clear();
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
            int hitNum = entity.hitBox.OverlapCollider(hitBoxFilter, hitResults);
            for (int i = 0; i < hitNum; i++)
            {
                Entity target = hitResults[i].GetComponentInParent<Entity>();
                if (target && !attatckedObjects.Contains(target))
                {
                    target.OnAttacked(new AttackInfo()
                    {
                        attacker = entity,
                        hitBox = entity.hitBox,
                        hurtBox = hitResults[i],
                        force = new Vector2(entity.FacingDirection, 0),
                    });
                    attatckedObjects.Add(target);
                }
            }
        }
    }


    public override void Exit()
    {
        entity.hitBoxEnabled = false;
        attatckedObjects.Clear();
        entity.attackCoolDownTimer = entity.attackCoolDownTime;
    }
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