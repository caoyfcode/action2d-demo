using UnityEngine;

public class AttackInfo
{
    static int nextId = 0;
    public int Id
    {
        get;
        private set;
    }
    public Entity attacker;
    public float damage;
    public Vector2 force;
    // public float poiseDamage; // 削韧;架势伤害
    public Collider2D hitBox;
    public Collider2D hurtBox;
    public HitType hitType = HitType.Unknown;

    public AttackInfo()
    {
        Id = nextId;
        nextId++;
    }

    public Vector2 AttackPoint
    {
        get
        {
            var attackerBB = hitBox.bounds;
            var attackedBB = hurtBox.bounds;
            var overlapBB = new Bounds
            {
                min = Vector2.Max(attackedBB.min, attackedBB.min),
                max = Vector2.Min(attackerBB.max, attackedBB.max),
            };

            return new Vector2(overlapBB.center.x, overlapBB.max.y);
        }
    }

}

public enum HitType
{
    Unknown,
    BodyCollision,
    Environment,
    EntityAttack,
}

// public enum DamageType
// {
//     Physics,
//     Magic,
// }