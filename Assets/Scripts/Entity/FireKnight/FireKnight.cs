using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireKnight : Entity
{
    // 玩家的 SummonAttack 攻击生效时长为 11 帧, 12/60=0.183s
    // 玩家的攻击前摇为 8 帧, 8/60=0.133s, 故无敌时间要超过 0.183s, 但短于 19/60=0.316s

    // TOOD 移入 status
    // internal float horizontalMoveSpeed = 6f;
    // float hitInvincibleTime = 0.3f;
    // float rollSpeed = 10f;
    const string ANIM_IDLE = "Idle";
    const string ANIM_RUN = "Run";
    const string ANIM_JUMP_UP = "JumpUp";
    const string ANIM_JUMP_DOWN = "JumpDown";
    const string ANIM_AIR_ATTACK = "AirAttack";
    const string ANIM_ROLL = "Roll";
    const string ANIM_ATTACK_1 = "Attack1";
    const string ANIM_ATTACK_2 = "Attack2";
    const string ANIM_ATTACK_3 = "Attack3";
    const string ANIM_SUPER_ATTACK = "SuperAttack";
    const string ANIM_DEFEND = "Defend";
    const string ANIM_TAKE_HIT = "TakeHit";
    const string ANIM_DEATH = "Death";
    

    public HitBox hitBox;
    public DefendBox defendBox;
    public Collider2D airAttackCollider;
    public Collider2D attack1Collider;
    public Collider2D attack2Collider;
    public Collider2D attack3Collider;
    public Collider2D superAttackCollider;
    public Collider2D rollAttackCollider;
    public Collider2D GetHitCollider(AttackType attackType) => attackType switch
    {
        AttackType.Attack1 => attack1Collider,
        AttackType.Attack2 => attack2Collider,
        AttackType.Attack3 => attack3Collider,
        AttackType.AirAttack => airAttackCollider,
        AttackType.SuperAttack => superAttackCollider,
        _ => null,
    };

    #region Command
    public class Command
    {
        public Vector2 speed = Vector2.zero;
        public bool roll = false;
        public bool defend = false;
        public AttackType attack = AttackType.None;

        public void Speed(float x, float y) => speed = new Vector2(x, y);
        public void SpeedX(float x) => speed.x = x;
        public void SpeedY(float y) => speed.y = y;
        public void Roll(bool value= true) => roll = value;
        public void Defend(bool value= true) => defend = value;
        public void Attack(AttackType attackType) => attack = attackType;
    }
    internal Command CurrentCommand { get; private set; }
    #endregion

    #region Attack Info
    // AttackState 使用的信息
    public enum AttackType
    {
        None,
        Attack1,
        Attack2,
        Attack3,
        AirAttack,
        SuperAttack,
    }
    #endregion

    #region Status
    public bool hitBoxEnabled = false; // 任何攻击都使用此, 由动画设定
    public bool animEnd = false; // 只在需要的动画里设置
    public bool animPreBreakable = false; // 前摇
    public bool animPostBreakable = false; // 后摇
    internal int airAttackCount = 0;
    internal float hitInvincibleTimer = 0;
    internal bool stateControlInvincible = false;
    
    public FireKnightData data;
    public override bool IsInvincible => hitInvincibleTimer > 0 || stateControlInvincible;
    public override CharacterData CharacterData => data; 
    #endregion

    #region StateMachine
    internal StateMachine<State> stateMachine;
    internal GroundFreeState groundFreeState;
    internal AirFreeState airFreeState;
    internal RollState rollState;
    internal AttackState attackState;
    internal DefendState defendState;
    internal TakeHitState takeHitState;
    internal DeathState deathState;
    #endregion


    protected override void Start()
    {
        base.Start();
        stateMachine = new StateMachine<State>();
        groundFreeState = new GroundFreeState(this, stateMachine);
        airFreeState = new AirFreeState(this, stateMachine);
        rollState = new RollState(this, stateMachine);
        attackState = new AttackState(this, stateMachine);
        defendState = new DefendState(this, stateMachine);
        takeHitState = new TakeHitState(this, stateMachine);
        deathState = new DeathState(this, stateMachine);
        stateMachine.Init(groundFreeState);
    }

    protected override void Update()
    {
        CurrentCommand = new Command();
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
            velocity.y -= gravity * Time.fixedDeltaTime;
            rb.velocity = velocity;
        }
    }

    void UpdateTimers()
    {
        if (hitInvincibleTimer > 0)
        {
            hitInvincibleTimer -= Time.deltaTime;
        }
    }

    public override void OnAttacked(HitInfo hitInfo)
    {
        // 防御状态只有背后能受到攻击
        if (IsInvincible || (stateMachine.CurrentState == defendState && hitInfo.force.x * FacingDirection < 0))
        {
            return;
        }
        base.OnAttacked(hitInfo);
        
        hitInvincibleTimer = data.hitInvincibleTime;
        data.heath -= hitInfo.damage;

        if (data.heath <= Mathf.Epsilon)
        {
            OnDeath();
        }
        else
        {
            if (stateMachine.CurrentState != attackState)
            {
                stateMachine.ChangeState(takeHitState);
            }
        }
    }

    public override void OnDeath()
    {
        base.OnDeath();
        stateMachine.ChangeState(deathState);
    }

    public abstract class State : IStateMachineState
    {
        protected FireKnight entity;
        protected StateMachine<State> stateMachine;
        public State(FireKnight fireKnight, StateMachine<State> machine)
        {
            entity = fireKnight;
            stateMachine = machine;
        }

        public virtual void Enter()
        {
            entity.animEnd = false;
            entity.animPreBreakable = false;
            entity.animPostBreakable = false;
        }
        public virtual bool Transition()
        {
            if (entity.isDead && this != entity.deathState)
            {
                entity.OnDeath();
                return true;
            }
            return false;
        }
        public abstract void Update();
        public virtual void Exit()
        {
            entity.animEnd = true;
            entity.animPreBreakable = false;
            entity.animPostBreakable = false;
        }
    }

    public class GroundFreeState : State
    {
        public GroundFreeState(FireKnight entity, StateMachine<State> machine) : base(entity, machine) 
        {
        }

        public override void Enter()
        {
            base.Enter();
            entity.airAttackCount = 0;
        }

        public override bool Transition()
        {
            if (base.Transition())
            {
                return true;
            }
            if (entity.CurrentCommand.roll)
            {
                stateMachine.ChangeState(entity.rollState);
                return true;
            }
            if (entity.CurrentCommand.attack != AttackType.None)
            {
                stateMachine.ChangeState(entity.attackState);
                return true;
            }
            if (entity.CurrentCommand.defend)
            {
                stateMachine.ChangeState(entity.defendState);
                return true;
            }
            if (entity.CurrentCommand.speed.y > 0 || !entity.isOnGround)
            {
                entity.isOnGround = false;
                stateMachine.ChangeState(entity.airFreeState);
                return true;
            }
            return false;
        }

        public override void Update()
        {
            entity.FacingDirection = entity.CurrentCommand.speed.x;
            entity.velocity.x = entity.CurrentCommand.speed.x;
            if (entity.CurrentCommand.speed.x != 0)
            {
                entity.animator.Play(ANIM_RUN);
            }
            else
            {
                entity.animator.Play(ANIM_IDLE);
            }
        }
    }

    public class AirFreeState : State
    {
        public AirFreeState(FireKnight entity, StateMachine<State> machine) : base(entity, machine) 
        {
        }

        public override bool Transition()
        {
            if (base.Transition())
            {
                return true;
            }
            if (entity.CurrentCommand.attack == AttackType.AirAttack && entity.airAttackCount == 0)
            {
                stateMachine.ChangeState(entity.attackState);
                return true;
            }
            if (entity.isOnGround && entity.velocity.y <= 0)
            {
                stateMachine.ChangeState(entity.groundFreeState);
                return true;
            }
            return false;
        }

        public override void Update()
        {
            entity.FacingDirection = entity.CurrentCommand.speed.x;
            entity.velocity.x = entity.CurrentCommand.speed.x;
            if (entity.CurrentCommand.speed.y != 0)
            {
                entity.velocity.y = entity.CurrentCommand.speed.y;
            }
            if (entity.velocity.y > 0)
            {
                entity.animator.Play(ANIM_JUMP_UP);
            }
            else if (entity.velocity.y < 0)
            {
                entity.animator.Play(ANIM_JUMP_DOWN);
            }
        }
    }

    public class RollState : State
    {
        public RollState(FireKnight entity, StateMachine<State> machine) : base(entity, machine) 
        {
        }

        public override void Enter()
        {
            base.Enter();
            entity.animEnd = false;
            entity.animator.Play(ANIM_ROLL);
            entity.hitBox.hitCollider = entity.rollAttackCollider;
        }

        public override bool Transition()
        {
            if (base.Transition())
            {
                return true;
            }
            if (entity.animEnd)
            {
                stateMachine.ChangeState(entity.groundFreeState);
                return true;
            }
            return false;
        }

        public override void Update()
        {
           entity.velocity.x = entity.FacingDirection * entity.data.rollSpeed;
           if (entity.hitBoxEnabled)
           {
                entity.hitBox.DoHit();  
           }
        }
    }

    public class AttackState : State
    {
        AttackType attackType = AttackType.Attack1;
        public AttackState(FireKnight entity, StateMachine<State> machine) : base(entity, machine) 
        {
        }

        public override void Enter()
        {
            base.Enter();
            entity.animEnd = false;
            if (stateMachine.PrevState == entity.airFreeState)
            {
                attackType = AttackType.AirAttack;
                entity.animator.Play(ANIM_AIR_ATTACK);
                entity.airAttackCount++;
                entity.velocity.x = 0;
                entity.velocity.y = 0;
            }
            else if (stateMachine.PrevState == entity.defendState)
            {
                attackType = AttackType.SuperAttack;
                entity.animator.Play(ANIM_SUPER_ATTACK);
            }
            else
            {
                attackType = entity.CurrentCommand.attack;
                switch (entity.CurrentCommand.attack)
                {
                    case AttackType.Attack1:
                        entity.animator.Play(ANIM_ATTACK_1);
                        break;
                    case AttackType.Attack2:
                        entity.animator.Play(ANIM_ATTACK_2);
                        break;
                    case AttackType.Attack3:
                        entity.animator.Play(ANIM_ATTACK_3);
                        break;
                }
            }
            entity.hitBox.hitCollider = entity.GetHitCollider(attackType);
        }

        public override bool Transition()
        {
            if (base.Transition())
            {
                return true;
            }
            if (entity.animEnd)
            {
                stateMachine.ChangeState(entity.groundFreeState);
                return true;
            }
            if (entity.animPostBreakable && attackType == AttackType.Attack1 && entity.CurrentCommand.attack == AttackType.Attack2)
            {
                stateMachine.ChangeState(entity.attackState);
                return true;
            }
            if (entity.animPostBreakable && attackType == AttackType.Attack2 && entity.CurrentCommand.attack == AttackType.Attack3)
            {
                stateMachine.ChangeState(entity.attackState);
                return true;
            }
            return false;
        }

        public override void Update()
        {
            // TODO 添加 GenHitInfo
            if (entity.hitBoxEnabled)
            {
                entity.hitBox.DoHit();
            }
        }

        public override void Exit()
        {
            base.Exit();
        }

        public AttackType AttackType => attackType;
    }

    public class DefendState : State
    {
        public DefendState(FireKnight entity, StateMachine<State> machine) : base(entity, machine) 
        {

        }

        public override void Enter()
        {
            base.Enter();
            entity.animEnd = false;
            entity.animator.Play(ANIM_DEFEND);
            entity.defendBox.StartDefend();
            entity.defendBox.onSuccess = OnDefendSuccess;
        }

        public override bool Transition()
        {
            if (base.Transition())
            {
                return true;
            }
            if (entity.animEnd)
            {
                stateMachine.ChangeState(entity.groundFreeState);
                return true;
            }
            return false;
        }

        public override void Update()
        {
            // nothing
        }

        public override void Exit()
        {
            base.Exit();
            entity.defendBox.StopDefend();
        }

        void OnDefendSuccess(HitInfo hitInfo)
        {
            if (hitInfo.force.x * entity.FacingDirection > 0)
            {
                return;
            }
            // TODO 播放打击特效
            stateMachine.ChangeState(entity.attackState);
        }
    }

    public class TakeHitState : State
    {
        public TakeHitState(FireKnight entity, StateMachine<State> machine) : base(entity, machine) 
        {
        }

        public override void Enter()
        {
            base.Enter();
            entity.animEnd = false;
            entity.animator.Play(ANIM_TAKE_HIT);
        }

        public override bool Transition()
        {
            if (base.Transition())
            {
                return true;
            }
            if (entity.animEnd)
            {
                stateMachine.ChangeState(entity.groundFreeState);
                return true;
            }
            return false;
        }

        public override void Update()
        {
            // nothing
        }

    }

    public class DeathState : State
    {
        public DeathState(FireKnight entity, StateMachine<State> machine) : base(entity, machine) 
        {

        }

        public override void Enter()
        {
            base.Enter();
            entity.animEnd = false;
            entity.animator.Play(ANIM_DEATH);
        }

        public override bool Transition() => false;

        public override void Update()
        {
            if (entity.animEnd)
            {
                Destroy(entity.gameObject);
            }
        }
    }
    
}
