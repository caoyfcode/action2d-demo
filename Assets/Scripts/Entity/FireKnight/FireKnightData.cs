using System;
using UnityEngine;

[Serializable]
public class FireKnightData : CharacterData
{
    [Header("Mutable Data")]
    public int stage = 1;
    [Header("Immutable Data")]
    public float jumpSpeed = 15f;
    public float attackComboCoolDownTime = 0.5f;
    public float airAttackCoolDownTime = 1.5f;
    public float horizontalMoveSpeed = 6f;
    public float hitInvincibleTime = 0.3f;
    public float rollSpeed = 10f;
}