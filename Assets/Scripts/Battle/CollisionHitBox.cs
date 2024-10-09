using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 类似于 HitBox 类但由于逻辑差异单独分开
public class CollisionHitBox : HitBox
{

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        targetFilter.SetLayerMask(LayerMask.GetMask("Player"));
        genHitInfoEntity = GenHitInfoEntity;
    }

    // Update is called once per frame
    void Update()
    {
        DoHit();
    }

    HitInfo GenHitInfoEntity(Entity target)
    {
        float diffX = target.transform.position.x - attacker.transform.position.x;
        float directionX = Mathf.Abs(diffX) > Mathf.Epsilon ? Mathf.Sign(diffX) : -target.FacingDirection;
        return new HitInfo()
        {
            attacker = attacker,
            damage = 1,
            force = new Vector2(directionX * 5, 0),
            hitBox = hitCollider,
            hitType = HitType.BodyCollision,
            hurtBox = hitResults[0],
            hitPoint = hitResults[0].bounds.center,
        };
    }
}
