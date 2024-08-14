using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    public Entity source;
    public ContactFilter2D targetFilter;
    public Collider2D hitBox;
    public int maxHitNum = 5; // 同时碰撞的最多数量
    internal Func<AttackInfo> GenAttackInfo = null; // 获取不带 hurtbox 的 attackInfo

    private HashSet<Entity> attackedEntities = new();
    private Collider2D[] hitResults;

    private bool hitStarted;

    protected virtual void Start()
    {
        hitStarted = false;
        hitResults = new Collider2D[maxHitNum];
    }

    public void StartHit()
    {
        hitStarted = true;
    }

    public void StopHit()
    {
        if (hitStarted)
        {
            attackedEntities.Clear();
        }
        hitStarted = false;
    }

    public void DoHit()
    {
        if (!hitStarted)
            return;
        int hitNum = hitBox.OverlapCollider(targetFilter, hitResults);
        for (int i = 0; i < hitNum; i++)
        {
            Entity target = hitResults[i].GetComponentInParent<Entity>();
            if (target && !attackedEntities.Contains(target))
            {
                attackedEntities.Add(target);
                var attackInfo = GenAttackInfo != null
                    ? GenAttackInfo()
                    : new AttackInfo()
                    {
                        attacker = source,
                        damage = 0,
                        force = new Vector2(source.FacingDirection, 0),
                        hitBox = hitBox
                    };
                attackInfo.hurtBox = hitResults[i];
                target.OnAttacked(attackInfo);
            }
        }
    }
}
