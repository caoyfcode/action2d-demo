using System;
using UnityEngine;

[Serializable]
public class FireKnightInfo : CharacterInfo
{
    [Header("FireKnight")]
    public int stage = 1;
    [Header("FireKnight Constant")]
    public float jumpSpeed = 20f;
    public float attackComboCoolDownTime = 0.5f;
    public float airAttackCoolDownTime = 1.5f;
    public float horizontalMoveSpeed = 6f;
    // 玩家的 SummonAttack 攻击生效时长为 11 帧, 12/60=0.183s
    // 玩家的攻击前摇为 8 帧, 8/60=0.133s, 故无敌时间要超过 0.183s, 但短于 19/60=0.316s
    public float hitInvincibleTime = 0.3f;
    public float rollSpeed = 10f;
}