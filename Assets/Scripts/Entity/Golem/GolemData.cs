using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GolemData : CharacterData
{
    [Header("Golem")]
    public int stage = 1;
    [Header("Constant")]
    internal float jumpSpeed = 5f;
    internal float moveSpeed = 5f;
    internal float hitInvincibleTime = 0.3f;
    internal float stunTime = 5f;
    internal int stunCount = 10;
    internal float jumpAttackCoolDownTime;
    // 因为要在代码里手动设置值, 所以不要用 public, 防止 unity 给覆盖掉
}
