using UnityEngine;

public class DeathBringer : Entity
{
    internal const string ANIM_IDLE = "Idle";
    internal const string ANIM_WALK = "Walk";
    internal const string ANIM_ATTACK = "Attack";
    internal const string ANIM_DEATH = "Death";
    internal float attackCoolDownTime = 1f; // 攻击结束后下一次攻击需要的时间
    internal float stunTime = 0.3f;
    float hitInvincibleTime = 0.3f;

    public HitBox hitBox;

    #region Command
    public class Command
    {
        public float moveX = 0;
        public bool attack = false;
        public Vector2 forceMove = Vector2.zero;
    }
    internal Command CurrentCommand {get; private set;}
    #endregion

    #region State Machine
    internal StateMachine<DeathBringerState> stateMachine;
    internal DeathBringerIdle idleState;
    internal DeathBringerWalk walkState;
    internal DeathBringerAttack attackState;
    internal DeathBringerStun stunState;
    internal DeathBringerDeath deathState;
    #endregion

    [Header("Status")]
    public bool hitBoxEnabled = false;
    public bool attackEnd = true;
    public bool deathEnd = false;
    public bool attackCancelable;
    internal float attackCoolDownTimer = 0;
    internal float hitInvincibleTimer = 0;
    public CharacterStatus status;
    public override bool IsInvincible => hitInvincibleTimer > 0;

    public void SetCommand(Command cmd)
    {
        CurrentCommand = cmd;
    }

    protected override void Start()
    {
        base.Start();
        hitFX = GetComponent<HitFX>();
        stateMachine = new StateMachine<DeathBringerState>();
        idleState = new DeathBringerIdle(this, stateMachine);
        walkState = new DeathBringerWalk(this, stateMachine);
        attackState = new DeathBringerAttack(this, stateMachine);
        stunState = new DeathBringerStun(this, stateMachine);
        deathState = new DeathBringerDeath(this, stateMachine);
        stateMachine.Init(idleState);
    }

    protected override void Update()
    {
        SetCommand(new Command());
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

    public override void OnAttacked(HitInfo hitInfo)
    {
        if (IsInvincible)
        {
            return;
        }
        base.OnAttacked(hitInfo);
        hitInvincibleTimer = hitInvincibleTime;
        status.heath -= hitInfo.damage;

        if (status.heath <= Mathf.Epsilon)
        {
            OnDeath();
        }
        else
        {
            if (stateMachine.CurrentState != attackState || attackCancelable)
            {
                stateMachine.ChangeState(stunState);
            }
        }
    }

    public override void OnDeath()
    {
        base.OnDeath();
        stateMachine.ChangeState(deathState);
    }
}