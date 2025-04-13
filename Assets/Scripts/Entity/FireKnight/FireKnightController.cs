using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AttackType = FireKnight.AttackType;

public class FireKnightController : EntityController
{
    FireKnight entity;

    // target detecting
    public float forwardDetectPoint;
    public float backwardDetectPoint;
    public float detectOffsetY;

    // checker
    public float groundHeight;

    public bool bossFightStarted = false;


    // [SerializeField]
    // private Entity target = null;
    // [SerializeField]
    // private Vector2 targetPosition = Vector2.zero;
    // private float keepTargetTimer = 0f;
    // public Entity Target => bossFightStarted ? Game.Player : (keepTargetTimer > 0f ? target : null); // 用于区分作为boss和普通怪物的行为
    private float waitTimer = 0f;
    private float airAttackCoolDownTimer = 0f;
    private float attackComboCoolDownTimer = 0f;
    private float defendCoolDownTimer = 0f;
    private BehaviourTree behaviourTree;

    void Awake()
    {
        entity = GetComponent<FireKnight>();
    }


    public override void Init()
    {
        entity.onTakeHit.AddListener(OnAttacked);
        BTNode attackCombo = new BTBuilder()
            .Sequence()
                .Fallback()
                    .Condition(() => DetectTargetInAttackRange(AttackType.Attack1))
                    .Action(() => MoveToTargetForAttack(AttackType.Attack1))
                .End()
                .WaitUntil(() => waitTimer <= 0f, () => waitTimer = Random.Range(0.1f, 0.2f))
                .Action(() => StartAttack(AttackType.Attack1))
                .WaitUntil(() => entity.animPostBreakable)
                .NeverFail()
                    .Sequence()
                        .Condition(() => DetectTargetInAttackRange(AttackType.Attack2))
                        .WaitUntil(() => waitTimer <= 0f, () => waitTimer = 0.1f)
                        .Action(() => StartAttack(AttackType.Attack2))
                        .WaitUntil(() => entity.animPostBreakable)
                        // .Condition(() => entity.status.stage == 2) // 检测2阶段
                        .WaitUntil(() => waitTimer <= 0f, () => waitTimer = 0.1f)
                        .Action(FaceTarget) // 转向行动
                        .Action(() => StartAttack(AttackType.Attack3))
                    .End()
                .End()
                .WaitUntil(() => entity.stateMachine.CurrentState != entity.attackState)
                .WaitUntil(() => waitTimer <= 0f, () => waitTimer = Random.Range(0.3f, 0.5f))
            .End()
            .BuildNode();
        BTNode jumpAndAttack = new BTBuilder()
            .Sequence()
                .Condition(TargetIsOnAir) // 目标是否在空中一定高度
                .Condition(() => entity.stateMachine.CurrentState == entity.groundFreeState)
                .NeverFail()
                    .Sequence()
                        .Action(StartJump)
                        .Action(WaitAirTopOrTarget())
                        .Action(() => StartAttack(AttackType.AirAttack))
                        .WaitUntil(() => entity.stateMachine.CurrentState != entity.attackState)
                    .End()
                .End()
                .Action(WaitAirToGround)
                .WaitUntil(() => entity.isOnGround)
            .End()
            .BuildNode();

        BTNode defendAndAttack = new BTBuilder()
            .Sequence()
                .Condition(() => 
                    DetectTargetInAttackRange(AttackType.SuperAttack)
                ) // 检测自己是否正在角色攻击范围且角色攻击前摇
                .Action(() => {
                    tryDenfendTimer = 0.2f;
                    return BTNodeStatus.Success;
                })
                .Action(StartDefend) // 开始防御
                .WaitUntil(() => entity.stateMachine.CurrentState != entity.defendState)
                // .NeverFail()
                //     .Sequence()
                //         .Action(() => BTNodeStatus.Running) // 检测防御结束或被攻击到
                //         .Action(() => BTNodeStatus.Success) // 被攻击到则开始反击
                //         .WaitUntil(() => entity.stateMachine.CurrentState != entity.attackState) // 直到攻击结束
                //     .End()
                // .End()
            .End()
            .BuildNode();
        BTNode rollAttack = new BTBuilder()
            .Sequence()
                .Condition(() => entity.info.stage == 2) // 检测2阶段
                .Condition(() => true) // 距离检测
                .Action(() => BTNodeStatus.Success) // 调整方向
                .Action(() => BTNodeStatus.Running) // 执行翻滚斩
            .End()
            .BuildNode();

        behaviourTree = new BTBuilder()
            .Sequence()
                // .Condition(DetectTarget)
                .Fallback()
                    .Sequence()
                        .Condition(() => defendCoolDownTimer <= 0f)
                        .Node(defendAndAttack)
                    .End()
                    .Sequence()
                        .Condition(() => airAttackCoolDownTimer <= 0f)
                        .Node(jumpAndAttack)
                        .WaitUntil(() => waitTimer <= 0f, () => waitTimer = Random.Range(0.1f, 0.3f))
                    .End()
                    .Sequence()
                        .Condition(() => attackComboCoolDownTimer <= 0f)
                        .Node(attackCombo)
                        .WaitUntil(() => waitTimer <= 0f, () => waitTimer = Random.Range(0.1f, 0.3f))
                    .End()
                .End()
            .End()
            .Build();
    }

    public override void OnStartBossFight()
    {
        bossFightStarted = true;
    }

    public void OnAttacked(HitInfo hitInfo)
    {
        entity.FacingDirection = Mathf.Sign(-hitInfo.force.x);
    }

    public void OnDeath()
    {
        
    }

    public override void UpdateCommand()
    {
        if (!bossFightStarted || !WorldManager.Player)
            return;
        UpdateTimers();
        if (!entity.isDead)
        {
            behaviourTree.Update();
        }
    }

    void UpdateTimers()
    {
        // if (keepTargetTimer > 0f)
        // {
        //     keepTargetTimer -= Time.deltaTime;
        // }
        if (waitTimer > 0f)
        {
            waitTimer -= Time.deltaTime;
        }
        if (airAttackCoolDownTimer > 0f)
        {
            airAttackCoolDownTimer -= Time.deltaTime;
        }
        if (attackComboCoolDownTimer > 0f)
        {
            attackComboCoolDownTimer -= Time.deltaTime;
        }
        if (defendCoolDownTimer > 0f)
            defendCoolDownTimer -= Time.deltaTime;
        if (tryDenfendTimer > 0f)
            tryDenfendTimer -= Time.deltaTime;
    }

    // bool DetectTarget()
    // {
    //     if (Target)
    //     {
    //         return true;
    //     }
    //     target = null;
    //     Vector3 back = transform.position + new Vector3(backwardDetectPoint * entity.FacingDirection, detectOffsetY, 0);
    //     var castHit = Physics2D.Raycast(back, new Vector2(entity.FacingDirection, 0), forwardDetectPoint - backwardDetectPoint,
    //         LayerMask.GetMask("Player"));
    //     if (castHit.collider == null)
    //     {
    //         return false;
    //     }
    //     else
    //     {
    //         target = castHit.collider.GetComponentInParent<Entity>();
    //         if (target == null)
    //             return false;
    //         targetPosition = target.transform.position;
    //         keepTargetTimer = 2f;
    //         Game.BossSystem.StartBossFight(entity);
    //         return true;
    //     }
    // }

    bool DetectTargetInAttackRange(AttackType attackType)
    {
        Debug.Log("DetectTargetInAttackRange " + attackType);
        if (!WorldManager.Player)
        {
            return false;
        }
        Collider2D hitCollider = entity.GetHitCollider(attackType);
        // if (hitCollider.IsTouching(WorldManager.Player.col))
        // {
        //     Debug.Log(true);
        //     return true;
        // }
        // 不用精细的 IsTouching 了，用 bounds 检测
        if (hitCollider.bounds.Intersects(WorldManager.Player.col.bounds))
        {
            Debug.Log(true);
            return true;
        }
        // 因为 Enemy 与 Player 无碰撞, 因而不使用 IsTouching
        if (entity.col.bounds.Intersects(WorldManager.Player.col.bounds))
        {
            Debug.Log(true);
            return true;
        }
        Debug.Log(false);
        return false;
    }

    BTNodeStatus MoveToTargetForAttack(AttackType attackType)
    {
        Debug.Log("MoveToTargetForAttack");
        if (!WorldManager.Player)
        {
            return BTNodeStatus.Failure;
        }
        if (DetectTargetInAttackRange(attackType))
        {
            return BTNodeStatus.Success;
        }
        float speedX = CalcSpeedXToTarget();
        entity.CurrentCommand.Speed(speedX, 0f);
        return BTNodeStatus.Running;
    }

    float CalcSpeedXToTarget()
    {
        if (!WorldManager.Player)
        {
            return 0f;
        }
        float diff = WorldManager.Player.transform.position.x - entity.transform.position.x;
        if (diff > 0)
        {
            diff = WorldManager.Player.col.bounds.min.x - entity.col.bounds.max.x;
            if (diff < WorldManager.Player.col.bounds.size.x)
            {
                diff = 0;
            }
        }
        else
        {
            diff = WorldManager.Player.col.bounds.max.x - entity.col.bounds.min.x;
            if (diff > -WorldManager.Player.col.bounds.size.x)
            {
                diff = 0;
            }
        }
        return Mathf.Sign(diff) * entity.info.horizontalMoveSpeed;
    }

    BTNodeStatus FaceTarget()
    {
        Debug.Log("FaceTarget");
        if (!WorldManager.Player)
        {
            return BTNodeStatus.Success;
        }
        float diff = WorldManager.Player.transform.position.x - entity.transform.position.x;
        entity.FacingDirection = Mathf.Sign(diff);
        return BTNodeStatus.Success;
    }

    BTNodeStatus StartAttack(AttackType attackType)
    {
        Debug.Log("StartAttack " + attackType);
        Debug.Log("Current state is " + entity.stateMachine.CurrentState);
        Debug.Log("Current attack state type is " + entity.attackState.AttackType);
        if (entity.stateMachine.CurrentState == entity.attackState && entity.attackState.AttackType == attackType)
        {
            Debug.Log("Start Success " + attackType);
            if (attackType == AttackType.Attack1)
            {
                attackComboCoolDownTimer = entity.info.attackComboCoolDownTime;
            }
            return BTNodeStatus.Success;
        }
        if (!entity.isDead)
        {
            entity.CurrentCommand.Attack(attackType);
            Debug.Log("Starting attack is " + attackType);
            return BTNodeStatus.Running;
        }
        Debug.Log(BTNodeStatus.Failure);
        return BTNodeStatus.Failure;
    }

    bool TargetIsOnAir()
    {
        if (!WorldManager.Player)
            return false;
        return WorldManager.Player.col.bounds.min.y > entity.col.bounds.min.y + 0.5f;
    }

    BTNodeStatus StartJump()
    {
        Debug.Log("StartJump");
        if (entity.stateMachine.CurrentState == entity.airFreeState)
        {
            airAttackCoolDownTimer = entity.info.airAttackCoolDownTime;
            return BTNodeStatus.Success;
        }
        else if (entity.stateMachine.CurrentState == entity.groundFreeState)
        {
            entity.CurrentCommand.SpeedY(entity.info.jumpSpeed);
            if (WorldManager.Player)
            {
                FaceTarget();
            }
            return BTNodeStatus.Running;
        }
        else
        {
            return BTNodeStatus.Failure;
        }
    }
    BTNodeStatus WaitAirTop()
    {
        // 到达最高点
        Debug.Log("entity.velocity.y " + entity.velocity.y);
        if (entity.velocity.y < 0)
        {
            Debug.Log("velocity y < 0");
            return BTNodeStatus.Success;
        }
        else if (entity.stateMachine.CurrentState != entity.airFreeState)
        {
            return BTNodeStatus.Failure;
        }
        else
        {
            entity.CurrentCommand.SpeedX(entity.FacingDirection * entity.info.horizontalMoveSpeed);
            return BTNodeStatus.Running;
        }
    }

    IEnumerable<BTNodeStatus> WaitAirTopOrTarget()
    {
        // 到达最高点或碰到主角攻击
        while (true)
        {
            if (entity.velocity.y < 0)
            {
                yield return BTNodeStatus.Success;
            }
            else if (entity.stateMachine.CurrentState != entity.airFreeState)
            {
                yield return BTNodeStatus.Failure;
            }
            else if (Mathf.Abs(WorldManager.Player.transform.position.y - entity.transform.position.y) < Mathf.Epsilon)
            {
                yield return BTNodeStatus.Success;
            }
            else
            {
                float speedX = CalcSpeedXToTarget();
                if (speedX * entity.FacingDirection < 0)
                {
                    speedX = 0;
                }
                entity.CurrentCommand.SpeedX(speedX);
                yield return BTNodeStatus.Running;
            }
        }
    }

    BTNodeStatus WaitAirToGround()
    {
        if (entity.stateMachine.CurrentState != entity.airFreeState)
        {
            return BTNodeStatus.Success;
        }
        entity.CurrentCommand.SpeedX(entity.FacingDirection * entity.info.horizontalMoveSpeed);
        return BTNodeStatus.Running;
    }

    private float tryDenfendTimer = 0;
    BTNodeStatus StartDefend()
    {
        Debug.Log("Start defend");
        if (entity.stateMachine.CurrentState == entity.defendState)
        {
            Debug.Log("Start Defend Success");
            defendCoolDownTimer = Random.Range(2f, 5f);
            return BTNodeStatus.Success;
        }
        if (!entity.isDead && tryDenfendTimer > 0)
        {
            entity.CurrentCommand.Defend(true);
            Debug.Log("Starting Defend");
            return BTNodeStatus.Running;
        }
        Debug.Log(BTNodeStatus.Failure);
        return BTNodeStatus.Failure;
    }

    void OnDrawGizmosSelected()
    {
        var entity = GetComponent<FireKnight>();
        // 检测范围
        Gizmos.color = Color.green;
        Vector3 front = transform.position + new Vector3(forwardDetectPoint * entity.FacingDirection, detectOffsetY, 0);
        Vector3 back = transform.position + new Vector3(backwardDetectPoint * entity.FacingDirection, detectOffsetY, 0);
        Gizmos.DrawLine(back, front);
    }
}
