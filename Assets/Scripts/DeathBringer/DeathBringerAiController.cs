
using System;
using UnityEngine;

public class DeathBringerAiController : MonoBehaviour
{
    private DeathBringer entity;
    private Entity target = null;
    private Vector2 targetPosition;
    public float attackDistance;

    public Collider2D targetDetector;
    private ContactFilter2D detectorFilter = new();
    private Collider2D[] detectResults = new Collider2D[5];

    void Start()
    {
        entity = GetComponent<DeathBringer>();
        detectorFilter.SetLayerMask(LayerMask.GetMask("Player"));
    }

    void Update()
    {
        if (entity.isDead)
        {
            entity.AddCommand(new DeathBringer.Command());
            return;
        }
        var cmd = new DeathBringer.Command();
        if (target == null && entity.stateMachine.CurrentState != entity.attackState)
        {
            int num = targetDetector.OverlapCollider(detectorFilter, detectResults);
            for (int i = 0; i < num; i++)
            {
                Entity t = detectResults[i].GetComponentInParent<Entity>();
                if (t && t is Warrior)
                {
                    target = t;
                    targetPosition = t.transform.position;
                }
            }
        }
        if (target != null)
        {
            float distance = targetPosition.x - entity.hitBox.bounds.center.x;
            float entityDistance = targetPosition.x - entity.transform.position.x;
            float moveX = 0;
            if (!entity.hitBox.bounds.Contains(targetPosition) && distance * entityDistance > 0)
            {
                moveX = Math.Sign(distance);
            }
            else
            {
                moveX = 0;
                if (entity.hitBox.bounds.Contains(target.transform.position))
                {
                    if (entity.stateMachine.CurrentState != entity.attackState)
                    {
                        cmd.attack = true;
                    }
                }
                target = null;
            }
            cmd.moveX = moveX;
        }
        entity.AddCommand(cmd);
    }
}