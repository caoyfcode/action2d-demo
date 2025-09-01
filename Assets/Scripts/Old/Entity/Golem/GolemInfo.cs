using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GolemInfo : CharacterInfo
{
    [Header("Golem")]
    public int stage = 1;
    [Header("Golem Constant")]
    internal float jumpSpeed = 5f;
    internal float moveSpeed = 5f;
    internal float hitInvincibleTime = 0.3f;
    internal float stunTime = 3f;
    internal int stunCount = 10;
    internal float jumpLandAttackCoolDownTime = 5f;
    internal float attackCoolDownTime = 2f;
    internal float maxSprintTime = 2f;
    internal float sprintSpeed = 12f;
    internal float sprintCoolDownTime = 5f;
    // 因为要在代码里手动设置值, 所以不要用 public, 防止 unity 给覆盖掉
}
