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


    [SerializeField]
    private Entity target = null;
    [SerializeField]
    private Vector2 targetPosition = Vector2.zero;
    private float keepTargetTimer = 0f;
    public Entity Target => keepTargetTimer > 0f ? target : null;
    private float waitTimer = 0f;
    // TODO 两个 cool down timer 移入 status
    private float airAttackCoolDownTimer = 0f;
    private float attackComboCoolDownTimer = 0f;
    private BehaviourTree behaviourTree;


    public override void Init()
    {
        entity = GetComponent<FireKnight>();
        entity.onAttacked.AddListener(OnAttacked);
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
                        .Action(WaitAirTop)
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
                .Condition(() => true) // 检测自己是否正在角色攻击范围且角色攻击前摇
                .Action(() => BTNodeStatus.Running) // 开始防御
                .NeverFail()
                    .Sequence()
                        .Action(() => BTNodeStatus.Running) // 检测防御结束或被攻击到
                        .Action(() => BTNodeStatus.Success) // 被攻击到则开始反击
                        .WaitUntil(() => entity.stateMachine.CurrentState != entity.attackState) // 直到攻击结束
                    .End()
                .End()
            .End()
            .BuildNode();
        BTNode rollAttack = new BTBuilder()
            .Sequence()
                .Condition(() => entity.data.stage == 2) // 检测2阶段
                .Condition(() => true) // 距离检测
                .Action(() => BTNodeStatus.Success) // 调整方向
                .Action(() => BTNodeStatus.Running) // 执行翻滚斩
            .End()
            .BuildNode();

        behaviourTree = new BTBuilder()
            .Sequence()
                .Condition(DetectTarget)
                .Fallback()
                    .Sequence()
                        .Condition(() => airAttackCoolDownTimer <= 0f)
                        .Node(jumpAndAttack)
                    .End()
                    .Sequence()
                        .Condition(() => attackComboCoolDownTimer <= 0f)
                        .Node(attackCombo)
                    .End()
                .End()
            .End()
            .Build();
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
        if (Target)
        {
            targetPosition = target.transform.position;
        }
        UpdateTimers();
        if (!entity.isDead)
        {
            behaviourTree.Update();
        }
    }

    void UpdateTimers()
    {
        if (keepTargetTimer > 0f)
        {
            keepTargetTimer -= Time.deltaTime;
        }
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
    }

    bool DetectTarget()
    {
        if (Target)
        {
            return true;
        }
        target = null;
        Vector3 back = transform.position + new Vector3(backwardDetectPoint * entity.FacingDirection, detectOffsetY, 0);
        var castHit = Physics2D.Raycast(back, new Vector2(entity.FacingDirection, 0), forwardDetectPoint - backwardDetectPoint,
            LayerMask.GetMask("Player"));
        if (castHit.collider == null)
        {
            return false;
        }
        else
        {
            target = castHit.collider.GetComponentInParent<Entity>();
            if (target == null)
                return false;
            targetPosition = target.transform.position;
            keepTargetTimer = 2f;
            Game.BossSystem.StartBossFight(entity);
            return true;
        }
    }

    bool DetectTargetInAttackRange(AttackType attackType)
    {
        Debug.Log("DetectTargetInAttackRange");
        Collider2D hitCollider = entity.GetHitCollider(attackType);
        if (hitCollider.IsTouching(target.col))
        {
            Debug.Log(true);
            return true;
        }
        // 因为 Enemy 与 Player 无碰撞, 因而不使用 IsTouching
        if (entity.col.bounds.Intersects(target.col.bounds))
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
        if (!Target)
        {
            return BTNodeStatus.Failure;
        }
        if (DetectTargetInAttackRange(attackType))
        {
            return BTNodeStatus.Success;
        }
        float diff = targetPosition.x - entity.transform.position.x;
        if (Mathf.Abs(diff) < Mathf.Epsilon)
        {
            diff = 0;
        }
        if (diff == 0)
        {
            return BTNodeStatus.Failure;
        }
        entity.CurrentCommand.Speed(Mathf.Sign(diff) * entity.data.horizontalMoveSpeed, 0f);
        return BTNodeStatus.Running;
    }

    BTNodeStatus FaceTarget()
    {
        Debug.Log("FaceTarget");
        if (!Target)
        {
            return BTNodeStatus.Success;
        }
        float diff = targetPosition.x - entity.transform.position.x;
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
                attackComboCoolDownTimer = entity.data.attackComboCoolDownTime;
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
        if (!target)
            return false;
        return target.col.bounds.min.y > groundHeight + 2;
    }

    BTNodeStatus StartJump()
    {
        Debug.Log("StartJump");
        if (entity.stateMachine.CurrentState == entity.airFreeState)
        {
            airAttackCoolDownTimer = entity.data.airAttackCoolDownTime;
            return BTNodeStatus.Success;
        }
        else if (entity.stateMachine.CurrentState == entity.groundFreeState)
        {
            entity.CurrentCommand.SpeedY(entity.data.jumpSpeed);
            if (Target)
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
        // 到达最高点攻击
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
            entity.CurrentCommand.SpeedX(entity.FacingDirection * entity.data.horizontalMoveSpeed);
            return BTNodeStatus.Running;
        }
    }

    BTNodeStatus WaitAirToGround()
    {
        if (entity.stateMachine.CurrentState != entity.airFreeState)
        {
            return BTNodeStatus.Success;
        }
        entity.CurrentCommand.SpeedX(entity.FacingDirection * entity.data.horizontalMoveSpeed);
        return BTNodeStatus.Running;
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
