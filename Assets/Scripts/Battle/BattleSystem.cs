using UnityEngine;

public class HitInfo
{
    static int nextId = 0;
    public int Id
    {
        get;
        private set;
    }

    public Entity projectileSource; // 若 attacker 是 projectile, 则此项为主人, 否则为 null
    public Entity attacker;
    public float damage;
    public Vector2 force;
    // public float poiseDamage; // 削韧;架势伤害
    public Collider2D hitBox;
    public HitType hitType = HitType.Unknown;

    // 以下由 xxxHitBox 填写
    public Collider2D hurtBox;
    public Vector2 hitPoint;

    public HitInfo()
    {
        Id = nextId;
        nextId++;
    }

}

public enum HitType
{
    Unknown,
    BodyCollision,
    Environment,
    EntityAttack,
    Projectile,
}

// public enum DamageType
// {
//     Physics,
//     Magic,
// }