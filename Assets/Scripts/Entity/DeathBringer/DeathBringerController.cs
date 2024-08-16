
using System;
using UnityEngine;

public class DeathBringerController : EntityController
{
    private DeathBringer entity;
    private Entity target = null;
    private Vector2 targetPosition;
    private float keepTargetTimer = 0f;

    public Collider2D targetDetector;
    private ContactFilter2D detectorFilter = new();
    private Collider2D[] detectResults = new Collider2D[5];

    public override void Init()
    {
        entity = GetComponent<DeathBringer>();
        detectorFilter.SetLayerMask(LayerMask.GetMask("Player"));
    }

    public override void UpdateCommand()
    {
        keepTargetTimer -= Time.deltaTime;
        if (entity.isDead || entity.stateMachine.CurrentState == entity.attackState)
        {
            entity.SetCommand(new DeathBringer.Command());
            return;
        }
        var cmd = new DeathBringer.Command();
        int num = targetDetector.OverlapCollider(detectorFilter, detectResults);

        if (num == 0) // 未检测到有目标, 则继续走还是停下
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
            for (int i = 0; i < num; i++)
            {
                Entity t = detectResults[i].GetComponentInParent<Entity>();
                if (t && t.CompareTag("Player"))
                {
                    SetTarget(t);
                    break;
                }
            }
            cmd.moveX = CalcMoveDirection();
            if (entity.hitBox.bounds.Contains(targetPosition) && entity.attackCoolDownTimer <= 0)
            {
                cmd.attack = true;
                target = null;
            }
        }
        entity.SetCommand(cmd);
    }


    public override void OnAttacked(AttackInfo attackInfo)
    {
        entity.HorizontalMoveAndFlip(Mathf.Sign(-attackInfo.force.x));
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
        float distance = targetPosition.x - entity.transform.position.x;
        if (Mathf.Abs(distance) <= 0.5f)
        {
            return 0;
        }

        return Mathf.Sign(distance);
    }
}