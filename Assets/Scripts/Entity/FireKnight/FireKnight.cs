using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireKnight : Entity
{
    float hitInvincibleTime = 0.3f;
    float rollSpeed = 10f;
    const string ANIM_IDLE = "Idle";
    const string ANIM_RUN = "Run";
    const string ANIM_JUMP_UP = "JumpUp";
    const string ANIM_JUMP_DOWN = "JumpDown";
    const string ANIM_AIR_ATTACK = "AirAttack";
    const string ANIM_ROLL = "Roll";
    const string ANIM_ATTACK_1 = "Attack1";
    const string ANIM_ATTACK_2_COMBO = "Attack2Combo";
    const string ANIM_ATTACK_3_COMBO = "Attack3Combo";
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

    #region Command
    public class Command
    {
        public Vector2 speed = Vector2.zero;
        public bool roll = false;
        public bool airAttack = false;
        public int attackCombo = 0;
        public bool defend = false;
        public AttackType AttackType
        {
            get
            {
                if (airAttack)
                    return AttackType.AirAttack;
                if (attackCombo > 0)
                    return AttackType.AttackCombo;
                return AttackType.None;
            }
        }
    }
    internal Command CurrentCommand { get; private set; }
    #endregion

    #region Attack Info
    // AttackState 使用的信息
    public enum AttackType
    {
        None,
        AirAttack,
        AttackCombo,
        SuperAttack,
    }
    #endregion

    #region Status
    public bool hitBoxEnabled = false; // 任何攻击都使用此, 由动画设定
    public bool animEnd = false; // 只在需要的动画里设置
    internal int airAttackCount = 0;
    internal float hitInvincibleTimer = 0;
    internal bool stateControlInvincible = false;
    
    public CharacterStatus status;
    public override bool IsInvincible => hitInvincibleTimer > 0 || stateControlInvincible;
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

    public void SetCommand(Command cmd)
    {
        CurrentCommand = cmd;
    }

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
        if (stateMachine.CurrentState == defendState && hitInfo.force.x * FacingDirection < 0)
        {
            return;
        }
        base.OnAttacked(hitInfo);
        
        hitInvincibleTimer = hitInvincibleTime;
        status.heath -= hitInfo.damage;

        var obj = Instantiate(Game.VFXLoader.bloodEffectPrefab);
        obj.transform.position = hitInfo.hitPoint;
        if (hitInfo.force.x < 0)
        {
            obj.transform.Rotate(180f, 0, 0f);
        }

        if (status.heath <= Mathf.Epsilon)
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

        public virtual void Enter() {}
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
        public virtual void Exit() {}
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
            if (entity.CurrentCommand.attackCombo > 0)
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
            entity.HorizontalMoveAndFlip(entity.CurrentCommand.speed.x);
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
            if (entity.CurrentCommand.airAttack && entity.airAttackCount == 0)
            {
                stateMachine.ChangeState(entity.attackState);
                return true;
            }
            if (entity.isOnGround)
            {
                stateMachine.ChangeState(entity.groundFreeState);
                return true;
            }
            return false;
        }

        public override void Update()
        {
            entity.HorizontalMoveAndFlip(entity.CurrentCommand.speed.x);
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
            entity.hitBox.hitBox = entity.rollAttackCollider;
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
           entity.velocity.x = entity.FacingDirection * entity.rollSpeed;
           if (entity.hitBoxEnabled)
           {
                entity.hitBox.StartHit();
                entity.hitBox.DoHit();  
           }
           else
           {
                entity.hitBox.StopHit();
           }
        }
    }

    public class AttackState : State
    {
        AttackType attackType = AttackType.AttackCombo;
        int totalCombo = 0;
        int currentCombo = 0;
        bool prevHitBoxEnabled = false;
        public AttackState(FireKnight entity, StateMachine<State> machine) : base(entity, machine) 
        {
        }

        public override void Enter()
        {
            base.Enter();
            entity.animEnd = false;
            prevHitBoxEnabled = false;
            currentCombo = 0;
            if (stateMachine.PrevState == entity.airFreeState)
            {
                attackType = AttackType.AirAttack;
                entity.animator.Play(ANIM_AIR_ATTACK);
                entity.airAttackCount++;
            }
            else if (stateMachine.PrevState == entity.defendState)
            {
                attackType = AttackType.SuperAttack;
                entity.animator.Play(ANIM_SUPER_ATTACK);
            }
            else
            {
                attackType = AttackType.AttackCombo;
                totalCombo = entity.CurrentCommand.attackCombo;
                switch (totalCombo)
                {
                    case 1:
                        entity.animator.Play(ANIM_ATTACK_1);
                        break;
                    case 2:
                        entity.animator.Play(ANIM_ATTACK_2_COMBO);
                        break;
                    case 3:
                        entity.animator.Play(ANIM_ATTACK_3_COMBO);
                        break;
                }
            }
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
            // TODO 分类别、数量控制移动
            bool justEnableHitBox = !prevHitBoxEnabled && entity.hitBoxEnabled;
            // TODO 添加 GenHitInfo
            if (justEnableHitBox)
            {
                currentCombo++;
                switch (attackType)
                {
                    case AttackType.AirAttack:
                        entity.hitBox.hitBox = entity.airAttackCollider;
                        break;
                    case AttackType.SuperAttack:
                        entity.hitBox.hitBox = entity.superAttackCollider;
                        break;
                    case AttackType.AttackCombo:
                        switch (currentCombo)
                        {
                            case 1:
                                entity.hitBox.hitBox = entity.attack1Collider;
                                break;
                            case 2:
                                entity.hitBox.hitBox = entity.attack2Collider;
                                break;
                            case 3:
                                entity.hitBox.hitBox = entity.attack3Collider;
                                break;
                        }
                        break;
                }
            }
            if (entity.hitBoxEnabled)
            {
                entity.hitBox.StartHit();
                entity.hitBox.DoHit();
            }
            else
            {
                entity.hitBox.StopHit();
            }
            prevHitBoxEnabled = entity.hitBoxEnabled;
        }

        public override void Exit()
        {
            base.Exit();
            prevHitBoxEnabled = false;
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
