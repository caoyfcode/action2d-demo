
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
            Destroy(gameObject);
            return;
        }
        base.Update();
        
        if (isOverlapEnvironment)
        {
            OnDeath();
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
        hitBox = hitBox.hitCollider,
        force = rb.velocity.normalized,
        hitType = HitType.Projectile,
    };

    public override void OnDeath()
    {
        base.OnDeath();
        rb.velocity = Vector2.zero;
    }

    public override void OnAttackSuccess(HitInfo hitInfo)
    {
        base.OnAttackSuccess(hitInfo);
        OnDeath();
    }
}
