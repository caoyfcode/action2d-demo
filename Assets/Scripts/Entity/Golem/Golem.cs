using System.Collections;
using System.Collections.Generic;
using NodeCanvas.StateMachines;
using Unity.VisualScripting;
using UnityEngine;

public class Golem : Entity
{
    const string ANIM_IDLE = "Idle";
    const string ANIM_ATTACK_1 = "Attack1";
    const string ANIM_ATTACK_2 = "Attack2";
    const string ANIM_ATTACK_3 = "Attack3";
    const string ANIM_DEATH = "Death";
    const string ANIM_START_JUMP = "StartJump";
    const string ANIM_JUMPING = "Jumping";
    const string ANIM_LAND = "Land";
    const string ANIM_WALK = "Walk";
    const string ANIM_STUN = "Stun"; // 晕倒, 硬直状态
    const string ANIM_SPRINT = "Sprint";

    public const string FSM_FREE = "Free";
    public const string FSM_STUN = "Stun";
    public const string FSM_ATTACK_1 = "Attack1";
    public const string FSM_ATTACK_2 = "Attack2";
    public const string FSM_ATTACK_3 = "Attack3";
    public const string FSM_JUMP = "Jump";
    public const string FSM_SPRINT = "Sprint";
    public const string FSM_LAND_ATTACK = "LandAttack";
    public const string FSM_DEATH = "Death";

    public FSMOwner fsm;
    public HitBox hitBox;
    public Collider2D attack1Collider;
    public Collider2D attack2Collider;
    public Collider2D attack3Collider;
    public Collider2D landAttackCollider;
    public HitBox sprintHitBox;
    public SpawnAfterImage spawnAfterImage;

    #region ControlState
    public class ControlState
    {
        public float speedX = 0;
    }
    public ControlState CurrentControl { get; private set; } = new();
    #endregion

    #region Status
    public bool hitBoxEnabled = false; // 近战 hitBox
    public bool animEnd;
    public bool animPreBreakable = false; // 前摇
    public bool animPostBreakable = false; // 后摇
    internal float hitInvincibleTimer = 0;
    public float stunCounter = 0;

    public GolemData data;
    public override CharacterData CharacterData => data; 
    public override bool IsInvincible => base.IsInvincible || hitInvincibleTimer > 0;
    #endregion

    protected override void Awake()
    {
        base.Awake();
        if (!fsm)
        {
            fsm = GetComponent<FSMOwner>();
        }
    }

    protected override void Update()
    {
        CurrentControl = new ControlState();
        base.Update();
        velocity = rb.velocity;
        fsm.UpdateBehaviour();
        UpdateTimers();
        MergeVelocity();
        rb.velocity = velocity;
    }

    protected virtual void FixedUpdate()
    {
        if (fsm.currentDeepStateName == FSM_FREE || fsm.currentDeepStateName == FSM_STUN)
        {
            var velocity = rb.velocity;
            velocity.y -= gravity * Time.fixedDeltaTime;
            if (velocity.y < -20)
                velocity.y = -20;
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

    public override void TakeHit(HitInfo hitInfo)
    {
        base.TakeHit(hitInfo);
        
        hitInvincibleTimer = data.hitInvincibleTime;
        data.health -= hitInfo.damage;
        stunCounter++;
        // AddForce(hitInfo.force);
        if (stunCounter >= data.stunCount && fsm.currentDeepStateName != FSM_STUN)
        {
            fsm.TriggerState(FSM_STUN);
            stunCounter = 0;
        }

        if (data.health <= Mathf.Epsilon)
        {
            Die();
        }
        else
        {
        }
    }

    public override void Die()
    {
        base.Die();
        fsm.TriggerState(FSM_DEATH);
    }

    #region StateMachine Functions
    public void EnterFree()
    {
        velocity.y = 0;
    }

    public void UpdateFree()
    {
        if (CurrentControl.speedX == 0)
        {
            animator.Play(ANIM_IDLE);
            velocity.x = 0;
        }
        else
        {
            animator.Play(ANIM_WALK);
            FacingDirection = CurrentControl.speedX;
            velocity.x = CurrentControl.speedX;
        }
    }

    public float stunTimer = 0f;
    public bool StunEnd => stunTimer <= 0;
    public void EnterStun()
    {
        animator.Play(ANIM_STUN);
        stunTimer = data.stunTime;
        velocity.x = 0;
        velocity.y = 0;
    }

    public void UpdateStun()
    {
        if (stunTimer > 0)
        {
            stunTimer -= Time.deltaTime;
        }
        velocity.x = 0;
        stunCounter = 0;
    }

    public void ExitStun()
    {
        stunTimer = 0f;
        stunCounter = 0;
    }

    public void EnterAttack13(bool attack1=true)
    {
        animEnd = false;
        animator.Play(attack1 ? ANIM_ATTACK_1 : ANIM_ATTACK_3);
        velocity.x = 0;
        velocity.y = 0;
        hitBox.hitCollider = attack1 ? attack1Collider : attack3Collider;
        hitBox.PreDiscreteHit();
    }

    public void UpdateAttack13()
    {
        if (hitBoxEnabled)
        {
            hitBox.DoHit();
        }
        velocity.x = 0;
    }

    public void ExitAttack13()
    {
        hitBox.hitCollider = null;
        hitBox.PostDiscreteHit();
    }

    public float attack2SpeedY = 0;
    public void EnterAttack2()
    {
        animEnd = false;
        animator.Play(ANIM_ATTACK_2);
        velocity.x = 0;
        velocity.y = 0;
        hitBox.hitCollider = attack2Collider;
        hitBox.PreDiscreteHit();
        attack2SpeedY = 0;
    }

    public void UpdateAttack2()
    {
        if (hitBoxEnabled)
        {
            hitBox.DoHit();
        }
        velocity.y = attack2SpeedY;
        if (velocity.y > 0)
        {
            velocity.x = CurrentControl.speedX;
        }
        else
        {
            velocity.x = 0;
        }
    }

    public void ExitAttack2()
    {
        hitBox.hitCollider = null;
        hitBox.PostDiscreteHit();
    }

    public bool jumpGiveSpeed = false;
    float jumpInitHeight = 0f;
    bool JumpHighEnough => transform.position.y - jumpInitHeight > 4;
    public void EnterJump()
    {
        animEnd = false;
        animator.Play(ANIM_START_JUMP);
        jumpInitHeight = transform.position.y;
        jumpGiveSpeed = false;
    }

    public void UpdateJump()
    {
        if (animEnd)
        {
            animator.Play(ANIM_JUMPING);
            animEnd = false;
        }
        if (jumpGiveSpeed)
        {
            velocity.x = CurrentControl.speedX;
            if (JumpHighEnough)
            {
                if (velocity.y > 0)
                {
                    velocity.y -= 10 * gravity * Time.deltaTime;
                }
                else if (velocity.y < 0)
                {
                    velocity.y += gravity * Time.deltaTime;
                }
            }
            else
            {
                velocity.y = 20;
            }
        }
    }

    public void ExitJump()
    {
        animEnd = false;
        jumpGiveSpeed = false;
    }

    bool land = false;
    public void EnterLandAttack()
    {
        animEnd = false;
        animator.Play(ANIM_JUMPING);
        land = false;
        velocity.y = -30;
        hitBox.hitCollider = landAttackCollider;
        hitBox.PreDiscreteHit();
        hitBox.genHitInfoEntity += GenLandAttackHitInfo;
    }

    public void UpdateLandAttack()
    {
        if (!isOnGround)
        {
            velocity.y = -30;
        }
        velocity.x = 0;
        if (isOnGround && !land)
        {
            land = true;
            animator.Play(ANIM_LAND);
            // velocity.y = 0;
            if (data.stage == 2)
            {
                // TODO 地波spawner发生地波攻击
            }
        }
        if (hitBoxEnabled)
        {
            hitBox.DoHit();
        }
    }

    public void ExitLandAttack()
    {
        hitBox.PostDiscreteHit();
        velocity.y = 0;
        hitBox.genHitInfoEntity -= GenLandAttackHitInfo;
    }

    HitInfo GenLandAttackHitInfo(Entity target) => new()
    {
        attacker = this,
        damage = 0,
        force = new Vector2(target.transform.position.x - transform.position.x > 0 ? 1 : -1, 0),
        hitCollider = landAttackCollider,
        hitType = HitType.EntityAttack,
    };

    public void EnterSprint()
    {
        sprintHitBox.hitContinuousType = HitBox.HitContinuousType.Continuous;
        animator.Play(ANIM_SPRINT);
    }

    public void UpdateSprint()
    {
        sprintHitBox.DoHit();
        velocity.y = 0;
        velocity.x = CurrentControl.speedX;
        if (velocity.x != 0 && !spawnAfterImage.Spawn)
        {
            spawnAfterImage.Spawn = true;
        }
        else if (velocity.x == 0 && spawnAfterImage.Spawn)
        {
            spawnAfterImage.Spawn = false;
        }
    }

    public void ExitSprint()
    {
        spawnAfterImage.Spawn = false;
    }

    public void EnterDeath()
    {
        animEnd = false;
        animator.Play(ANIM_DEATH);
        velocity.x = 0;
        velocity.y = 0;
    }

    public void UpdateDeath()
    {
        velocity.x = 0;
        velocity.y = 0;
        if (animEnd)
        {
            DestoryEntity();
        }
    }
    #endregion
}
