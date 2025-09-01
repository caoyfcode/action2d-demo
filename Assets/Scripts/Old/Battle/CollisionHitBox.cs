using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 类似于 HitBox 类但由于逻辑差异单独分开
public class CollisionHitBox : HitBox
{

    public float damage = 1;

    public enum HitDirection
    {
        HorizontalTargetVerticalUp,
        HitBoxLocalUp,
    }
    public HitDirection hitDirection = HitDirection.HorizontalTargetVerticalUp;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        if (genHitInfoEntity == null)
            genHitInfoEntity = GenHitInfoEntity;
        hitContinuousType = HitContinuousType.Continuous;
    }

    // Update is called once per frame
    void Update()
    {
        hitContinuousType = HitContinuousType.Continuous;
        if (attacker && !attacker.isDead)
            DoHit();
    }

    HitInfo GenHitInfoEntity(Entity target)
    {
        float diffX = target.transform.position.x - attacker.transform.position.x;
        float directionX = Mathf.Abs(diffX) > Mathf.Epsilon ? Mathf.Sign(diffX) : -target.FacingDirection;
        var force = hitDirection switch {
            HitDirection.HorizontalTargetVerticalUp => new Vector3(directionX * 3, 1),
            _ => transform.up * 3,
        };
        return new HitInfo()
        {
            attacker = attacker,
            damage = damage,
            force = force,
            hitCollider = hitCollider,
            hitType = HitType.BodyCollision,
            hurtCollider = hitResults[0],
            hitPoint = hitResults[0].bounds.center,
        };
    }
}
