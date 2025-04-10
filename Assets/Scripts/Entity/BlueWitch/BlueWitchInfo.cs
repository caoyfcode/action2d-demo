using System;
using UnityEngine;

[Serializable]
public class BlueWitchInfo : CharacterInfo
{
    [Header("BlueWitch Constant")]
    internal float horizontalMoveSpeed = 8f;
    internal float jumpSpeed = 12f;
    internal float airJumpSpeed = 12f;
    internal float maxJumpTime = 0.37f;
    internal float minKeepDirectionTime = 0.15f; // 爬墙跳后 keepDirection, 直到到达最小保持时间且输入反向或跳跃结束
    internal float dashTime = 0.2f;
    internal float dashSpeed = 25f;
    internal float dashCoolDownTime = 0.5f; // 0.2+0.3 相邻两次 dash 的冷却时间
    internal float wallSlideSpeed = 2f;
    internal float attackComboTime = 0.3f;
    internal int maxAirAttackCount = 2; // 依据召唤的替身攻击类型设置空中攻击次数
    internal float hitInvincibleTime = 0.5f;
    internal float hitStunTime = 0.3f; // 短于受击无敌时间，因而只要受击一定可以硬直
}
