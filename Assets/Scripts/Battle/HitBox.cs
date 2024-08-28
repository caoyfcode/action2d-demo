using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    public Entity attacker;
    public ContactFilter2D targetFilter;
    public int maxHitNum = 5; // 同时碰撞的最多数量

    public Collider2D hitBox;
    internal Func<HitInfo> genHitInfo = null; // 获取不带 hurtbox 的 HitInfo

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

    public int DoHit()
    {
        if (!hitStarted)
            return 0;
        int hitNum = hitBox.OverlapCollider(targetFilter, hitResults);
        int num = 0;
        for (int i = 0; i < hitNum; i++)
        {
            Entity target = hitResults[i].GetComponentInParent<Entity>();
            if (target && !target.IsInvincible && !attackedEntities.Contains(target))
            {
                num++;
                attackedEntities.Add(target);
                var hitInfo = genHitInfo != null
                    ? genHitInfo()
                    : new HitInfo
                    {
                        attacker = attacker,
                        damage = 0,
                        force = new Vector2(attacker.FacingDirection, 0),
                        hitBox = hitBox,
                        hitType = HitType.EntityAttack,
                    };
                hitInfo.hurtBox = hitResults[i];
                hitInfo.hitPoint = hitInfo.hurtBox.bounds.center;
                hitInfo.attacker?.OnAttackSuccess(hitInfo);
                target.OnAttacked(hitInfo);
            }
        }
        return num;
    }
}
