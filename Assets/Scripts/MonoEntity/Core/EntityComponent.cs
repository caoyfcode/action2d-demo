using System;
using UnityEngine;

// EntityComponent 用于实现 Entity 的功能, 为了实现简单，不允许动态挂载/卸载, 不过可以 enabled
public class EntityComponent : EntityUpdateBase
{
    [NonSerialized]
    public MonoEntity entity;
    // 用 MonoBehaviour 的 .enable 了, 回调函数直接使用 MonoBehaviour 的函数即可
    // [SerializeField]
    // [Tooltip("影响各种 Update 的调用")]
    // private bool componentEnabled = true;
    // public bool ComponentEnabled
    // {
    //     get
    //     {
    //         return componentEnabled;
    //     }
    //     set
    //     {
    //         if (value && !componentEnabled)
    //         {
    //             componentEnabled = value;
    //             OnComponentEnable();
    //         }
    //         else if (!value && componentEnabled)
    //         {
    //             componentEnabled = value;
    //             OnComponentDisable();
    //         }
    //         componentEnabled = value;
    //     }
    // }
    //
    // protected virtual void OnComponentEnable() {}
    // protected virtual void OnComponentDisable() {}
}