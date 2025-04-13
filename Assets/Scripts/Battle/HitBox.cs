using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    public Entity attacker;
    public ContactFilter2D defendFilter;
    public ContactFilter2D targetFilter;
    public int maxHitNum = 5; // 同时碰撞的最多数量

    public Collider2D hitCollider;
    internal Func<HitInfo> genHitInfo = null; // 获取不带 hurtbox 的 HitInfo
    internal Func<Entity, HitInfo> genHitInfoEntity = null;
    internal Func<DefendBox, HitInfo> genHitInfoDefendBox = null;
    protected Collider2D[] hitResults;

    public HitContinuousType hitContinuousType = HitContinuousType.Discrete;
    protected HashSet<Entity> hurtEntities = new(); // 在离散类型攻击中

    // TODO 改名为 DoHitType/DoDamageType
    public enum HitContinuousType
    {
        Discrete, // 一次攻击
        Continuous, // 持续性攻击
    }

    protected virtual void Start()
    {
        hitResults = new Collider2D[maxHitNum];
    }

    // 离散情况需要手动调用
    public void PreDiscreteHit()
    {
        hurtEntities.Clear();
    }

    // 离散情况需要手动调用
    public void PostDiscreteHit()
    {
        hurtEntities.Clear();
    }

    public int DoHit()
    {
        int defendtNum = hitCollider.OverlapCollider(defendFilter, hitResults);
        for (int i = 0; i < defendtNum; i++)
        {
            DefendBox defendBox = hitResults[i].GetComponent<DefendBox>();
            if (!defendBox)
            {
                continue;
            }
            var hitInfo = genHitInfoDefendBox?.Invoke(defendBox) ?? genHitInfo?.Invoke() ?? new HitInfo
            {
                attacker = attacker,
                damage = 0,
                force = new Vector2(attacker.FacingDirection, 0),
                hitCollider = hitCollider,
                hitType = HitType.EntityAttack,
            };
            hitInfo.hurtCollider = hitResults[i];
            hitInfo.hitPoint = hitResults[i].bounds.center;
            if (defendBox && defendBox.DefendCheck(hitInfo))
            {
                defendBox.Defend(hitInfo);
            }
        }
        int hitNum = hitCollider.OverlapCollider(targetFilter, hitResults);
        int num = 0;
        for (int i = 0; i < hitNum; i++)
        {
            Entity target = hitResults[i].GetComponentInParent<Entity>();
            if (!target || (hitContinuousType == HitContinuousType.Discrete && hurtEntities.Contains(target)))
            {
                continue;
            }
            var hitInfo = genHitInfoEntity?.Invoke(target) ?? genHitInfo?.Invoke() ?? new HitInfo
            {
                attacker = attacker,
                damage = 0,
                force = new Vector2(attacker.FacingDirection, 0),
                hitCollider = hitCollider,
                hitType = HitType.EntityAttack,
            };
            hitInfo.hurtCollider = hitResults[i];
            hitInfo.hitPoint = hitResults[i].bounds.center;
            if (target && target.TakeHitCheck(hitInfo))
            {
                num++;
                if (hitContinuousType == HitContinuousType.Discrete)
                {
                    hurtEntities.Add(target);
                }
                target.TakeHit(hitInfo);
                attacker.HitSuccess(hitInfo);
            }
        }
        return num;
    }
}
