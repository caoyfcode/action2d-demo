using UnityEngine;

public class DeathBringer : Entity
{
    internal const string ANIM_IDLE = "Idle";
    internal const string ANIM_WALK = "Walk";
    internal const string ANIM_ATTACK = "Attack";
    internal const string ANIM_DEATH = "Death";
    internal float attackCoolDownTime = 0.5f; // 攻击结束后下一次攻击需要的时间

    [Header("Component")]
    public Collider2D hitBox;

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
    internal DeathBringerDeath deathState;
    #endregion

    [Header("Status")]
    public bool hitBoxEnabled = false;
    public bool attackEnd = true;
    public bool deathEnd = false;
    internal float attackCoolDownTimer = 0;

    public void SetCommand(Command cmd)
    {
        CurrentCommand = cmd;
    }

    protected override void Start()
    {
        base.Start();
        hitFX = GetComponent<HitFlashFX>();
        stateMachine = new StateMachine<DeathBringerState>();
        idleState = new DeathBringerIdle(this, stateMachine);
        walkState = new DeathBringerWalk(this, stateMachine);
        attackState = new DeathBringerAttack(this, stateMachine);
        deathState = new DeathBringerDeath(this, stateMachine);
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
        stateMachine.Update();
        MergeVelocity();
        rb.velocity = velocity;
    }

    protected virtual void FixedUpdate()
    {
        if (stateMachine.CurrentState == idleState || stateMachine.CurrentState == walkState)
        {
            var velocity = rb.velocity;
            velocity.y -= gravity * Time.fixedDeltaTime;
            rb.velocity = velocity;
        }
    }

    public override void OnAttacked(AttackInfo attackInfo)
    {
        base.OnAttacked(attackInfo);
    }
}