using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AttackType = FireKnight.AttackType;

public class FireKnightController : EntityController
{
    FireKnight entity;

    public float forwardDetectPoint;
    public float backwardDetectPoint;
    public float detectOffsetY;

    [SerializeField]
    private Entity target = null;
    [SerializeField]
    private Vector2 targetPosition = Vector2.zero;
    private float keepTargetTimer = 0f;
    private float attackWaitTimer = 0f;
    private BehaviourTree behaviourTree;

    public override void Init()
    {
        entity = GetComponent<FireKnight>();
        behaviourTree = new BTBuilder()
            .Sequence()
                .Condition(DetectTarget)
                .Fallback()
                    .Condition(() => DetectTargetInAttackRange(AttackType.Attack1))
                    .Action(() => MoveToTargetForAttack(AttackType.Attack1))
                .End()
                .WaitUntil(() => attackWaitTimer <= 0f, () => attackWaitTimer = Random.Range(0.1f, 0.2f))
                .Action(() => StartAttack(AttackType.Attack1))
                .WaitUntil(() => entity.animPostBreakable)
                .Sequence()
                    .Condition(() => DetectTargetInAttackRange(AttackType.Attack2))
                    .Action(() => StartAttack(AttackType.Attack2))
                .End()
                .WaitUntil(() => entity.stateMachine.CurrentState != entity.attackState)
                .WaitUntil(() => attackWaitTimer <= 0f, () => attackWaitTimer = Random.Range(0.3f, 0.5f))
            .End()
            .Build();
    }

    public override void OnAttacked(HitInfo hitInfo)
    {
        entity.HorizontalMoveAndFlip(Mathf.Sign(-hitInfo.force.x));
    }

    public override void OnDeath()
    {
        
    }

    public override void UpdateCommand()
    {
        if (keepTargetTimer > 0 && target != null)
        {
            targetPosition = target.transform.position;
        }
        var cmd = new FireKnight.Command();
        entity.SetCommand(cmd);
        UpdateTimers();
        behaviourTree.Update();
        
    }

    void UpdateTimers()
    {
        if (keepTargetTimer > 0f)
        {
            keepTargetTimer -= Time.deltaTime;
        }
        if (attackWaitTimer > 0f)
        {
            attackWaitTimer -= Time.deltaTime;
        }
    }

    bool DetectTarget()
    {
        if (target && keepTargetTimer > 0f)
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
        // TODO 因为 Enemy 与 Player 无碰撞, 因而若要使用 IsTouching 需要添加额外的碰撞伤害 trigger
        if (entity.col.IsTouching(target.col))
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
        if (keepTargetTimer <= 0 || target == null)
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
        var cmd = new FireKnight.Command()
        {
            speed = new Vector2(Mathf.Sign(diff) * entity.horizontalMoveSpeed, 0f)
        };
        entity.SetCommand(cmd);
        return BTNodeStatus.Running;
    }

    BTNodeStatus StartAttack(AttackType attackType)
    {
        Debug.Log("StartAttack " + attackType);
        Debug.Log("Current state is " + entity.stateMachine.CurrentState);
        Debug.Log("Current attack state type is " + entity.attackState.AttackType);
        if (entity.stateMachine.CurrentState == entity.attackState && entity.attackState.AttackType == attackType)
        {
            Debug.Log(BTNodeStatus.Success);
            return BTNodeStatus.Success;
        }
        if (!entity.isDead)
        {
            entity.SetCommand(new FireKnight.Command()
            {
                attack = attackType
            });
            Debug.Log(BTNodeStatus.Running);
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
