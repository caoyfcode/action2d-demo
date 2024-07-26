using UnityEngine;

public class DeathBringer : Entity
{
    internal const string ANIM_IDLE = "Idle";
    internal const string ANIM_WALK = "Walk";
    internal const string ANIM_ATTACK = "Attack";
    internal const string ANIM_DEATH = "Death";

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
    private bool gotNewCommand = false;
    #endregion

    #region State Machine
    internal StateMachine<DeathBringerState> stateMachine;
    internal DeathBringerIdle ildeState;
    internal DeathBringerWalk walkState;
    internal DeathBringerAttack attackState;
    internal DeathBringerDeath deathState;
    #endregion

    #region Animation
    #endregion

    [Header("Status")]
    public bool hitBoxEnabled = false;
    public bool attackCancelable = true;
    public bool attackEnd = true;
    public bool deathEnd = false;
    internal float attackCoolDownTimer = 0;

    public void AddCommand(Command cmd)
    {
        CurrentCommand = cmd;
        gotNewCommand = true;
    }

    protected override void Start()
    {
        base.Start();
        hitFX = GetComponent<HitFlashFX>();
        stateMachine = new StateMachine<DeathBringerState>();
        ildeState = new DeathBringerIdle(this, stateMachine);
        walkState = new DeathBringerWalk(this, stateMachine);
        attackState = new DeathBringerAttack(this, stateMachine);
        deathState = new DeathBringerDeath(this, stateMachine);
        stateMachine.Init(ildeState);
    }

    protected override void Update()
    {
        base.Update();
        if (!gotNewCommand)
        {
            CurrentCommand = new Command();
        }
        velocity = rb.velocity;
        if (attackCoolDownTimer > 0)
        {
            attackCoolDownTimer = -Time.deltaTime;
        }
        stateMachine.Update();
        gotNewCommand = false;
        rb.velocity = velocity;
    }

    protected override void FixedUpdate()
    {
        base.Update();
        if (stateMachine.CurrentState == ildeState || stateMachine.CurrentState == walkState)
        {
            var velocity = rb.velocity;
            velocity.y -= gravity * Time.deltaTime;
            rb.velocity = velocity;
        }
    }

    public override void OnAttacked()
    {
        base.OnAttacked();
    }
}