using System;

[Serializable]
public class FireKnightStatus : CharacterStatus
{
    public float jumpSpeed = 15f;
    public int stage = 1;
    public float attackComboCoolDownTime = 0.5f;
    public float airAttackCoolDownTime = 1.5f;
}