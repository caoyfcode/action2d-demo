
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DeathBringerData : CharacterData
{
    [Header("DeathBringer Constant")]
    public float attackCoolDownTime = 1f; // 攻击结束后下一次攻击需要的时间
    public float stunTime = 0.3f;
    public float hitInvincibleTime = 0.3f;
    public float horizontalMoveSpeed = 6f;
}