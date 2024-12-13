using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 继承 MonoBehaviour 而非使用接口是因为要让 Entity 自动获取
// Controller 的一切函数都由 Entity 主动调用
public abstract class EntityController : MonoBehaviour
{
    public abstract void Init();
    public abstract void UpdateCommand();
    // 作为 boss 时有用
    public virtual void OnStartBossFight() {}
}

// TODO 之后改为下面方法所表示的
// public interface IAgent
// {
//     void Init();
//     void UpdateCommand();
// }

// public abstract class CharacterAgent<T> : MonoBehaviour, IAgent where T : Entity
// {
//     public T entity;
//     public abstract void Init();
//     public abstract void UpdateCommand();
// }
