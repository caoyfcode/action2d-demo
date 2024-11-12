using UnityEngine;

public class DeathBringer : Entity
{
    internal const string ANIM_IDLE = "Idle";
    internal const string ANIM_WALK = "Walk";
    internal const string ANIM_ATTACK = "Attack";
    internal const string ANIM_DEATH = "Death";

    public HitBox hitBox;

    #region Command
    public class Command
    {
        public float speedX = 0;
        public bool attack = false;
        public void SpeedX(float speedX) => this.speedX = speedX;
        public void Attack(bool attack=true) => this.attack = attack;
    }
    internal Command CurrentCommand {get; private set;}
    #endregion

    #region State Machine
    internal StateMachine<State> stateMachine;
    internal IdleState idleState;
    internal WalkState walkState;
    internal AttackState attackState;
    internal StunState stunState;
    internal DeathState deathState;
    #endregion

    [Header("Status")]
    public bool hitBoxEnabled = false;
    // TODO 合并为 animEnd, animPreBreakable, animPostBreakable
    public bool attackEnd = true;
    public bool deathEnd = false;
    public bool attackCancelable;
    internal float attackCoolDownTimer = 0;
    internal float hitInvincibleTimer = 0;
    public DeathBringerData data;
    public override bool IsInvincible => base.IsInvincible || hitInvincibleTimer > 0;

    protected override void Start()
    {
        CurrentCommand = new Command();
        base.Start();
        entityFX = GetComponent<EntityFX>();
        stateMachine = new StateMachine<State>();
        idleState = new IdleState(this, stateMachine);
        walkState = new WalkState(this, stateMachine);
        attackState = new AttackState(this, stateMachine);
        stunState = new StunState(this, stateMachine);
        deathState = new DeathState(this, stateMachine);
        stateMachine.Init(idleState);
    }

    protected override void Update()
    {
        base.Update();
        velocity = rb.velocity;
        if (attackCoolDownTimer > 0)
        {
            attackCoolDownTimer -= Time.deltaTime;
        }
        if (hitInvincibleTimer > 0)
        {
            hitInvincibleTimer -= Time.deltaTime;
        }
        stateMachine.Update();
        MergeVelocity();
        rb.velocity = velocity;
    }

    protected virtual void FixedUpdate()
    {
        if (stateMachine.CurrentState == idleState || stateMachine.CurrentState == walkState || stateMachine.CurrentState == stunState)
        {
            var velocity = rb.velocity;
            velocity.y -= gravity * Time.fixedDeltaTime;
            rb.velocity = velocity;
        }
    }

    public override void TakeHit(HitInfo hitInfo)
    {
        base.TakeHit(hitInfo);
        hitInvincibleTimer = data.hitInvincibleTime;
        data.heath -= hitInfo.damage;
        AddForce(hitInfo.force);

        if (data.heath <= Mathf.Epsilon)
        {
            Die();
        }
        else
        {
            if (stateMachine.CurrentState != attackState || attackCancelable)
            {
                stateMachine.ChangeState(stunState);
            }
        }
    }

    public override void Die()
    {
        base.Die();
        stateMachine.ChangeState(deathState);
    }

    public abstract class State : IStateMachineState
    {
        protected DeathBringer entity;
        protected StateMachine<State> stateMachine;

        public State(DeathBringer deathBringer, StateMachine<State> machine)
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

    public class IdleState : State
    {
        public IdleState(DeathBringer deathBringer, StateMachine<State> machine) : base(deathBringer, machine)
        {
        }

        public override void Enter()
        {
            entity.animator.Play(ANIM_IDLE);
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
            if (entity.CurrentCommand.speedX != 0)
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

    public class WalkState : State
    {
        public WalkState(DeathBringer deathBringer, StateMachine<State> machine) : base(deathBringer, machine)
        {
        }

        public override void Enter()
        {
            entity.animator.Play(ANIM_WALK);
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
            if (entity.CurrentCommand.speedX == 0)
            {
                stateMachine.ChangeState(entity.idleState);
                return true;
            }
            return false;
        }

        public override void Update()
        {
            entity.velocity.x = entity.CurrentCommand.speedX;
            entity.FacingDirection = entity.CurrentCommand.speedX;
        }
    }

    public class AttackState : State
    {
        public AttackState(DeathBringer deathBringer, StateMachine<State> machine) : base(deathBringer, machine)
        {
        }

        public override void Enter()
        {
            entity.attackEnd = false;
            entity.velocity.x = 0;
            entity.velocity.y = 0;
            entity.animator.Play(ANIM_ATTACK);
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
            entity.attackCoolDownTimer = entity.data.attackCoolDownTime;
        }

        private HitInfo GenHitInfo() => new()
        {
            attacker = entity,
            damage = 1,
            force = new Vector2(entity.FacingDirection, 0),
            hitCollider = entity.hitBox.hitCollider,
            hitType = HitType.EntityAttack,
        };
    }

    public class StunState : State
    {
        private float timer;
        public StunState(DeathBringer deathBringer, StateMachine<State> machine) : base(deathBringer, machine)
        {
        }

        public override void Enter()
        {
            base.Enter();
            timer = entity.data.stunTime;
            entity.animator.Play(ANIM_IDLE);
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

    public class DeathState : State
    {
        public DeathState(DeathBringer deathBringer, StateMachine<State> machine) : base(deathBringer, machine)
        {
        }

        public override void Enter()
        {
            entity.animator.Play(ANIM_IDLE);
            entity.velocity.x = 0;
            entity.velocity.y = 0;
            entity.DestoryEntity();
        }

        public override bool Transition()
        {
            return false;
        }

        public override void Update() 
        {
            // if (entity.deathEnd)
            // {
            //     Destroy(entity.gameObject);
            // }
        }
    }
}