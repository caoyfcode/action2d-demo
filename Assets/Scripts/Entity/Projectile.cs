
public class Projectile : Entity
{
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
        base.Update();
        if (IsFacingWall)
        {
            Destroy(gameObject);
            return;
        }
        if (hitBox.genAttackInfo == null)
        {
            hitBox.genAttackInfo = GenAttackInfo;
        }

        hitBox.hitBox = col;
        hitBox.StartHit();
        if (hitBox.DoHit() > 0)
        {
            Destroy(gameObject);
        }
        rb.velocity = velocity;
    }

    protected virtual AttackInfo GenAttackInfo() => new AttackInfo()
    {
        attacker = hitBox.source,
        damage = damage,
        hitBox = hitBox.hitBox,
        force = rb.velocity.normalized,
    };
}
