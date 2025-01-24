using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CharacterInfo
{
    [Header("Common")]
    // 主角血量要取整, 敌人血量保持浮点数
    public float maxHealth;
    public float health;

    public float maxMagic;
    public float magic;

    // 攻击大部分为 1
    public float attack;
    public float defence;

}
