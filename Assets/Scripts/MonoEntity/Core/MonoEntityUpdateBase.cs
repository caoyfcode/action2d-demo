using System;
using UnityEngine;

// 其实如果不是为了方便与其它插件关联，完全没必要继承 Mono, 只要有 SubClass 即可
public abstract class EntityUpdateBase : MonoBehaviour
{
    public virtual void OnEntityAwake() {}
    public virtual void OnEntityEnable() {}
    public virtual void OnEntityStart() {}
    
    
    public virtual void PreEntityUpdate() {}
    public virtual void OnEntityUpdate() {}
    public virtual void PostEntityUpdate() {}
    
    public virtual void PreEntityFixedUpdate() {}
    public virtual void OnEntityFixedUpdate() {}
    public virtual void PostEntityFixedUpdate() {}
    
    public virtual void PreEntityLateUpdate() {}
    public virtual void OnEntityLateUpdate() {}
    public virtual void PostEntityLateUpdate() {}
    
    public virtual void OnEntityDisable() {}
	public virtual void PreEntityDestroy() {}
    public virtual void OnEntityDestroy() {}


    private bool phaseFlagCalculated = false;
    private PhaseFlag phaseFlag;
    public PhaseFlag PhaseFlag
    {
        get
        {
            if (!phaseFlagCalculated)
            {
                phaseFlag = CalculatePhaseFlag();
                phaseFlagCalculated = true;
            }
            return phaseFlag;
        }
    }
    // private static EntityUpdateBase dummyObj = new();
    private PhaseFlag CalculatePhaseFlag()
    {
        Action selfOnEntityAwake = OnEntityAwake;
        Action selfOnEntityEnable = OnEntityEnable;
        Action selfOnEntityStart = OnEntityStart;
        Action selfPreEntityUpdate = PreEntityUpdate;
        Action selfOnEntityUpdate = OnEntityUpdate;
        Action selfPostEntityUpdate = PostEntityUpdate;
        Action selfPreEntityFixedUpdate = PreEntityFixedUpdate;
        Action selfOnEntityFixedUpdate = OnEntityFixedUpdate;
        Action selfPostEntityFixedUpdate = PostEntityFixedUpdate;
        Action selfPreEntityLateUpdate = PreEntityLateUpdate;
        Action selfOnEntityLateUpdate = OnEntityLateUpdate;
        Action selfPostEntityLateUpdate = PostEntityLateUpdate;
        Action selfOnEntityDisable = OnEntityDisable;
        Action selfPreEntityDestroy = PreEntityDestroy;
        Action selfOnEntityDestroy = OnEntityDestroy;
        
        var phaseFlag = PhaseFlag.None;
        // DeclaringType 指的是所在的 class 的类型, class 不同说明覆写了
        if (selfOnEntityAwake.Method.DeclaringType != typeof(EntityUpdateBase))
        {
            phaseFlag |= PhaseFlag.OnEntityAwake;
        }
        if (selfOnEntityEnable.Method.DeclaringType != typeof(EntityUpdateBase))
        {
            phaseFlag |= PhaseFlag.OnEntityEnable;
        }
        if (selfOnEntityStart.Method.DeclaringType != typeof(EntityUpdateBase))
        {
            phaseFlag |= PhaseFlag.OnEntityStart;
        }
        if (selfPreEntityUpdate.Method.DeclaringType != typeof(EntityUpdateBase))
        {
            phaseFlag |= PhaseFlag.PreEntityUpdate;
        }
        if (selfOnEntityUpdate.Method.DeclaringType != typeof(EntityUpdateBase))
        {
            phaseFlag |= PhaseFlag.OnEntityUpdate;
        }
        if (selfPostEntityUpdate.Method.DeclaringType != typeof(EntityUpdateBase))
        {
            phaseFlag |= PhaseFlag.PostEntityUpdate;
        }
        if (selfPreEntityFixedUpdate.Method.DeclaringType != typeof(EntityUpdateBase))
        {
            phaseFlag |= PhaseFlag.PreEntityFixedUpdate;
        }
        if (selfOnEntityFixedUpdate.Method.DeclaringType != typeof(EntityUpdateBase))
        {
            phaseFlag |= PhaseFlag.OnEntityFixedUpdate;
        }
        if (selfPostEntityFixedUpdate.Method.DeclaringType != typeof(EntityUpdateBase))
        {
            phaseFlag |= PhaseFlag.PostEntityFixedUpdate;
        }
        if (selfPreEntityLateUpdate.Method.DeclaringType != typeof(EntityUpdateBase))
        {
            phaseFlag |= PhaseFlag.PreEntityLateUpdate;
        }
        if (selfOnEntityLateUpdate.Method.DeclaringType != typeof(EntityUpdateBase))
        {
            phaseFlag |= PhaseFlag.OnEntityLateUpdate;
        }
        if (selfPostEntityLateUpdate.Method.DeclaringType != typeof(EntityUpdateBase))
        {
            phaseFlag |= PhaseFlag.PostEntityLateUpdate;
        }
        if (selfOnEntityDisable.Method.DeclaringType != typeof(EntityUpdateBase))
        {
            phaseFlag |= PhaseFlag.OnEntityDisable;
        }

        if (selfPreEntityDestroy.Method.DeclaringType != typeof(EntityUpdateBase))
        {
            phaseFlag |= PhaseFlag.PreEntityDestroy;
        }
        if (selfOnEntityDestroy.Method.DeclaringType != typeof(EntityUpdateBase))
        {
            phaseFlag |= PhaseFlag.OnEntityDestroy;
        }

        return phaseFlag;
    }
}

[Flags]
public enum PhaseFlag : uint
{
    None = 0,

    OnEntityAwake = 1,
    OnEntityEnable = 1 << 1,
    OnEntityStart = 1 << 2,
    
    PreEntityUpdate = 1 << 3,
    OnEntityUpdate = 1 << 4,
    PostEntityUpdate = 1 << 5,
        
    PreEntityFixedUpdate =  1 << 6,
    OnEntityFixedUpdate = 1 << 7,
    PostEntityFixedUpdate = 1 << 8,
    
    PreEntityLateUpdate =  1 << 9,
    OnEntityLateUpdate = 1 << 10,
    PostEntityLateUpdate = 1 << 11,
    
    OnEntityDisable =  1 << 12,
	PreEntityDestroy = 1 << 13,
    OnEntityDestroy = 1 << 13,
   
}

public static class EnumFlagsExtension
{
    public static bool Contains(this PhaseFlag self, PhaseFlag other)
    {
        return (self & other) == other;
    }
}