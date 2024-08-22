
using System;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class DeathBringerController : EntityController
{
    public float forwardDetectPoint;
    public float backwardDetectPoint;
    public float detectOffsetY;
    public float attackMinPoint;
    public float attackMaxPoint;

    private DeathBringer entity;
    private Entity target = null;
    private Vector2 targetPosition;
    private float keepTargetTimer = 0f;

    public override void Init()
    {
        entity = GetComponent<DeathBringer>();
    }

    public override void UpdateCommand()
    {
        keepTargetTimer -= Time.deltaTime;
        if (entity.isDead || entity.stateMachine.CurrentState == entity.attackState || entity.stateMachine.CurrentState == entity.stunState)
        {
            entity.SetCommand(new DeathBringer.Command());
            return;
        }
        var cmd = new DeathBringer.Command();
        Vector3 back = transform.position + new Vector3(backwardDetectPoint * entity.FacingDirection, detectOffsetY, 0);
        var castHit = Physics2D.Raycast(back, new Vector2(entity.FacingDirection, 0), forwardDetectPoint - backwardDetectPoint,
            LayerMask.GetMask("Player"));

        if (castHit.collider == null) // 未检测到有目标, 则继续走还是停下
        {
            if (keepTargetTimer > 0)
            {
                cmd.moveX = CalcMoveDirection();
            }
            else
            {
                cmd.moveX = 0;
            }
            target = null;
        }
        else // 有目标, 计算前进还是攻击
        {
            SetTarget(castHit.collider.GetComponentInParent<Entity>());
            cmd.moveX = CalcMoveDirection();
            if (cmd.moveX == 0 && entity.attackCoolDownTimer <= 0)
            {
                cmd.attack = true;
                target = null;
            }
        }
        entity.SetCommand(cmd);
    }


    public override void OnAttacked(HitInfo hitInfo)
    {
        entity.HorizontalMoveAndFlip(Mathf.Sign(-hitInfo.force.x));
    }

    public override void OnDeath()
    {

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
        if (distance >= attackMinPoint && distance <= attackMaxPoint)
        {
            return 0;
        }

        return Mathf.Sign(diff);
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