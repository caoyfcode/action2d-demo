using UnityEngine;

public class HitInfo
{
    public Entity projectileSource; // 若 attacker 是 projectile, 则此项为主人, 否则为 null
    public Entity attacker;
    public float damage;
    public Vector2 force;
    // public float poiseDamage; // 削韧;架势伤害
    public Collider2D hitCollider;
    public HitType hitType = HitType.Unknown;

    // 以下必须由 HitBox 类填写
    public Collider2D hurtCollider;
    public Vector2 hitPoint;
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