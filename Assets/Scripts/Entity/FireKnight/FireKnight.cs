using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireKnight : Entity
{
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
    public bool animDefendCouldAttack = true; // 防御到后是否可以反击
    internal int airAttackCount = 0;
    internal float hitInvincibleTimer = 0;
    internal float defendSuccInvincibleTimer = 0;
    internal bool stateControlInvincible = false;
    
    public FireKnightInfo info;
    public override bool IsInvincible => base.IsInvincible || hitInvincibleTimer > 0 || stateControlInvincible || defendSuccInvincibleTimer > 0;
    public override CharacterInfo CharacterInfo => info; 
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
            if (velocity.y <= 0)
            {
                velocity.y -= gravity * Time.fixedDeltaTime;
            }
            else
            {
                velocity.y -= 1.5f * gravity * Time.fixedDeltaTime;
            }
            rb.velocity = velocity;
        }
    }

    void UpdateTimers()
    {
        if (hitInvincibleTimer > 0)
        {
            hitInvincibleTimer -= Time.deltaTime;
        }
        if (defendSuccInvincibleTimer > 0)
        {
            defendSuccInvincibleTimer -= Time.deltaTime;
        }
    }

    // 防御状态只有背后能受到攻击
    public override bool TakeHitCheck(HitInfo hitInfo) => base.TakeHitCheck(hitInfo) && !(stateMachine.CurrentState == defendState && hitInfo.force.x * FacingDirection <= 0);

    public override void TakeHit(HitInfo hitInfo)
    {
        base.TakeHit(hitInfo);
        
        hitInvincibleTimer = info.hitInvincibleTime;
        info.health -= hitInfo.damage;
        AddForce(hitInfo.force);

        if (info.health <= Mathf.Epsilon)
        {
            Die();
        }
        else
        {
            if (info.health <= info.maxHealth / 2)
            {
                info.stage = 2;
            }
            if (stateMachine.CurrentState != attackState) // defendState 也是可以被打断的
            {
                stateMachine.ChangeState(takeHitState);
            }
        }
    }

    public override void Die()
    {
        base.Die();
        WorldManager.CurrentLevel.DynamicGenerattePickupItem(
            item: new() {
                    id = ItemData.FireKnightItemId,
                    quantity = 1,
            },
            position: transform.position,
            velocity: new Vector2(0f, 15f)
        );
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
                entity.Die();
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
            entity.hitBox.PreDiscreteHit();
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
           entity.velocity.x = entity.FacingDirection * entity.info.rollSpeed;
           if (entity.hitBoxEnabled)
           {
                entity.hitBox.DoHit();  
           }
        }

        public override void Exit()
        {
            base.Exit();
            entity.hitBox.PostDiscreteHit();
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
            entity.hitBox.PreDiscreteHit();
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
                entity.hitBox.genHitInfoEntity = GenHitInfo;
                entity.hitBox.DoHit();
            }
        }

        public override void Exit()
        {
            base.Exit();
            entity.hitBox.PostDiscreteHit();
        }

        public AttackType AttackType => attackType;

        public HitInfo GenHitInfo(Entity target)
        {
            Vector2 force = new Vector2(entity.FacingDirection, 1);
            float damage = 1f;
            float diff = Mathf.Sign(target.transform.position.x - entity.transform.position.x);
            switch (attackType) {
                case AttackType.Attack2:
                    force = new(diff, 1);
                    break;
                case AttackType.SuperAttack:
                    damage = 2f;
                    break;
                default:
                    break;
            }
            return new() {
                attacker = entity,
                damage = damage,
                force = force,
                hitCollider = entity.hitBox.hitCollider,
                hitType = HitType.EntityAttack,
            };
        }
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
            entity.defendBox.defendCheck = DefendCheck;
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

        bool DefendCheck(HitInfo hitInfo)
        {
            return hitInfo.force.x * entity.FacingDirection < 0;
        }

        void OnDefendSuccess(HitInfo hitInfo)
        {
            if (hitInfo.force.x * entity.FacingDirection > 0)
            {
                return;
            }
            entity.defendSuccInvincibleTimer = 0.5f;
            if (entity.animDefendCouldAttack)
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
                entity.DestoryEntity();
            }
        }
    }
    
}
