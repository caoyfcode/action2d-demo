using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonEntity : Entity
{
    public Entity source;
    public float damage;

    public ContactFilter2D targetCastFilter = new();
    Collider2D[] hitResults = new Collider2D[10];

    protected int AttackAsProjectile()
    {
        int num = col.OverlapCollider(targetCastFilter, hitResults);

        for (int i = 0; i < num; i++)
        {
            Entity target = hitResults[i].GetComponentInParent<Entity>();
            if (target != null)
            {
                target.OnAttacked(new AttackInfo()
                {
                    attacker = source,
                    hitBox = col,
                    damage = damage,
                    direction = rb.velocity.normalized,
                    hurtBox = hitResults[i],
                }); ;
            }
        }
        return num;
    }
}
