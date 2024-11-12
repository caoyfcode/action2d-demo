
using UnityEngine;

public class Projectile : Entity
{
    public Entity source;
    public float damage;
    public HitBox hitBox;

    protected override void Start()
    {
        base.Start();
        FacingDirection = velocity.x;
        rb.velocity = velocity;
    }

    // 子类继续 override Update() 函数来编写路径
    protected override void Update()
    {
        if (isDead)
        {
            DestoryEntity();
            return;
        }
        base.Update();
        
        if (isOverlapEnvironment)
        {
            Die();
            return;
        }
        if (hitBox.genHitInfo == null)
        {
            hitBox.genHitInfo = GenHitInfo;
        }

        hitBox.hitCollider = col;
        hitBox.DoHit();
        rb.velocity = velocity;
    }

    protected virtual HitInfo GenHitInfo() => new()
    {
        projectileSource = source,
        attacker = this,
        damage = damage,
        hitCollider = hitBox.hitCollider,
        force = rb.velocity.normalized,
        hitType = HitType.Projectile,
    };

    public override void Die()
    {
        base.Die();
        rb.velocity = Vector2.zero;
    }

    public override void HitSuccess(HitInfo hitInfo)
    {
        base.HitSuccess(hitInfo);
        Die();
    }
}
