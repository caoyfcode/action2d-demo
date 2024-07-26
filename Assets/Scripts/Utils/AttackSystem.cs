using UnityEngine;

// TODO, 在 OnAttack 作参数传入
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
    public Vector2 direction;
    // public float poiseDamage; // 削韧;架势伤害
    Collider2D attackerCollider;
    Collider2D attackedCollider;

    public AttackInfo()
    {
        Id = nextId;
        nextId++;
    }
}