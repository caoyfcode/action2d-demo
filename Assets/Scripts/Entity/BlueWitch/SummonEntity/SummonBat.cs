using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonBat : SummonEntity
{
    const float speed = 30f;
    public Vector2 direction;

    protected override void Start()
    {
        base.Start();
        horizontalMoveSpeed = speed;
        FacingDirection = direction.x > 0 ? 1 : -1;
        rb.velocity = speed * direction;
    }

    protected override void Update()
    {
        base.Update();
        if (IsFacingWall)
        {
            Destroy(gameObject);
        }
        if (AttackAsProjectile() > 0)
        {
            Destroy(gameObject);
        }
    }
}
