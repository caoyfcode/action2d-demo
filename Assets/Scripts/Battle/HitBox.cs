using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    public Entity attacker;
    public ContactFilter2D targetFilter;
    public int maxHitNum = 5; // 同时碰撞的最多数量

    public Collider2D hitCollider;
    internal Func<HitInfo> genHitInfo = null; // 获取不带 hurtbox 的 HitInfo
    internal Func<Entity, HitInfo> genHitInfoEntity = null;
    internal Func<DefendBox, HitInfo> genHitInfoDefendBox = null;
    protected Collider2D[] hitResults;

    protected virtual void Start()
    {
        hitResults = new Collider2D[maxHitNum];
    }

    public int DoHit()
    {
        int hitNum = hitCollider.OverlapCollider(targetFilter, hitResults);
        int num = 0;
        for (int i = 0; i < hitNum; i++)
        {
            DefendBox defendBox = hitResults[i].GetComponent<DefendBox>();
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
        for (int i = 0; i < hitNum; i++)
        {
            Entity target = hitResults[i].GetComponentInParent<Entity>();
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
                target.TakeHit(hitInfo);
                attacker.HitSuccess(hitInfo);
            }
        }
        return num;
    }
}
