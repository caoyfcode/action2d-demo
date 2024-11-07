
using System;
using UnityEngine;

public class DeathBringerController : EntityController
{
    // attackMinPoint < 0 attackMaxPoint > 0 可以避免左右横跳
    public float forwardDetectPoint;
    public float backwardDetectPoint;
    public float detectOffsetY;
    public float attackMinPoint;
    public float attackMaxPoint;

    private DeathBringer entity;
    private Entity target = null;
    private Vector2 targetPosition;
    private float keepTargetTimer = 0f;
    private BehaviourTree behaviourTree;

    public override void Init()
    {
        entity = GetComponent<DeathBringer>();
        entity.onAttacked.AddListener(OnAttacked);
        behaviourTree = new BTBuilder()
            .Sequence()
                .Condition(DetectTartget)
                .Fallback()
                    .Condition(DetectTargetInRange)
                    .Action(MoveToTarget)
                .End()
                .Action(AttackTarget)
            .End()
            .Build();
    }

    public override void UpdateCommand()
    {
        keepTargetTimer -= Time.deltaTime;
        if (!entity.isDead)
            behaviourTree.Update();
    }

    bool DetectTartget()
    {
        if (entity.isDead || entity.stateMachine.CurrentState == entity.attackState)
            return false;
        if (target != null && keepTargetTimer > 0)
            return true;
        Vector3 back = transform.position + new Vector3(backwardDetectPoint * entity.FacingDirection, detectOffsetY, 0);
        var castHit = Physics2D.Raycast(back, new Vector2(entity.FacingDirection, 0), forwardDetectPoint - backwardDetectPoint,
            LayerMask.GetMask("Player"));
        if (castHit.collider == null)
        {
            return false;
        }
        else
        {
            var target = castHit.collider.GetComponentInParent<Entity>();
            if (target == null)
                return false;
            SetTarget(target);
            return true;
        }
    }

    bool DetectTargetInRange()
    {
        return CalcMoveDirection() == 0;
    }

    BTNodeStatus MoveToTarget()
    {
        if ((entity.stateMachine.CurrentState != entity.idleState 
            && entity.stateMachine.CurrentState != entity.walkState) 
            || keepTargetTimer < 0)
        {
            return BTNodeStatus.Failure;
        }
        var moveX = CalcMoveDirection();
        entity.CurrentCommand.SpeedX(moveX * entity.data.horizontalMoveSpeed);
        if (moveX == 0)
            return BTNodeStatus.Success;
        else
            return BTNodeStatus.Running;
    }

    BTNodeStatus AttackTarget()
    {
        if (entity.stateMachine.CurrentState == entity.attackState)
        { // 成功开始攻击
            return BTNodeStatus.Success;
        }
        else if (!entity.isDead && entity.attackCoolDownTimer <= 0)
        { // 尝试攻击
            entity.CurrentCommand.Attack();
            return BTNodeStatus.Running;
        }
        return BTNodeStatus.Failure;
    }


    public void OnAttacked(HitInfo hitInfo)
    {
        entity.FacingDirection = Mathf.Sign(-hitInfo.force.x);
    }

    private void SetTarget(Entity t)
    {
        target = t;
        targetPosition = target.transform.position;
        keepTargetTimer = 0.5f;
    }

    private float CalcMoveDirection()
    {
        float diff = targetPosition.x - entity.transform.position.x;
        float distance = diff * entity.FacingDirection;
        // attackMinPoint < 0 attackMaxPoint > 0 可以避免左右横跳
        if (distance >= attackMinPoint && distance <= attackMaxPoint)
        {
            return 0;
        }

        if (distance < attackMinPoint)
        {
            return -entity.FacingDirection;
        }
        else
        {
            return entity.FacingDirection;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (UnityEditor.Selection.activeGameObject != gameObject)
            return;
        var entity = GetComponent<DeathBringer>();
        // 检测范围
        Gizmos.color = Color.green;
        Vector3 front = transform.position + new Vector3(forwardDetectPoint * entity.FacingDirection, detectOffsetY, 0);
        Vector3 back = transform.position + new Vector3(backwardDetectPoint * entity.FacingDirection, detectOffsetY, 0);
        Gizmos.DrawLine(back, front);
        Gizmos.color = Color.red;
        Vector3 max = transform.position + new Vector3(attackMaxPoint * entity.FacingDirection, detectOffsetY, 0);
        Vector3 min = transform.position + new Vector3(attackMinPoint * entity.FacingDirection, detectOffsetY, 0);
        Gizmos.DrawLine(min, max);
    }
#endif
}