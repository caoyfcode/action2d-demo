using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 继承 MonoBehaviour 而非使用接口是因为要让 Entity 自动获取
// Controller 的一切函数都由 Entity 主动调用
public abstract class EntityController : MonoBehaviour
{
    public abstract void Init();
    public abstract void UpdateCommand();
    public abstract void OnAttacked(AttackInfo attackInfo);
    public abstract void OnDeath();
}
