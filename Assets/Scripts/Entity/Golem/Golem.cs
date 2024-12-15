using System.Collections;
using System.Collections.Generic;
using NodeCanvas.StateMachines;
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
    const string ANIM_Walk = "Walk";
    const string ANIM_Stun = "Stun"; // 晕倒, 硬直状态

    const string FSM_FREE = "Free";

    public FSMOwner fsm;

    #region ControlState
    public class ControlState
    {
        public float speedX = 0;
        public bool startJump = false;
        public bool jumping = false;
        public bool land = false;
        public bool attack1 = false;
        public bool attack2 = false;
        public bool attack3 = false;
    }
    public ControlState CurrentControl { get; private set; }
    #endregion

    #region Status
    public bool hitBoxEnabled = false; // 近战 hitBox
    public bool animEnd;
    public bool animPreBreakable = false; // 前摇
    public bool animPostBreakable = false; // 后摇
    internal float hitInvincibleTimer = 0;

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
        if (fsm.currentDeepStateName == FSM_FREE)
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

    #region StateMachine Functions
    public void EnterFree()
    {
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
            animator.Play(ANIM_Walk);
            FacingDirection = CurrentControl.speedX;
            velocity.x = CurrentControl.speedX;
        }
    }

    public float stunTimer = 0f;
    public void EnterStun()
    {
        animator.Play(ANIM_Stun);
        stunTimer = data.stunTime;
    }

    public void UpdateStun()
    {
        if (stunTimer > 0)
        {
            stunTimer -= Time.deltaTime;
        }
        else
        {
        }
    }

    public void ExitStun()
    {
        
    }
    #endregion
}
