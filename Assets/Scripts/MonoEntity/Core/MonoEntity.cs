using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.Serialization;

public class MonoEntity : MonoBehaviour
{
    #region EntityComponent
     
    // 不标成 readonly 是为了在 Inspector 的 Debug 模式中只读显示
    private List<EntityComponent> components = new();
    private Dictionary<Type, EntityComponent> componentsDic = new();
    private Dictionary<PhaseFlag, List<EntityComponent>> phaseComponents = new();
    public T GetEntityComponent<T>() where T : EntityComponent => componentsDic.ContainsKey(typeof(T)) ? (T)componentsDic[typeof(T)] : null;
    public bool HasComponent<T>() where T : EntityComponent => componentsDic.ContainsKey(typeof(T));
    #endregion
    
    public int id = -1;
    
    public void RefreshEntityComponents()
    {
        // collect all components
        components.Clear();
        componentsDic.Clear();
        phaseComponents.Clear();

        var gotComponents = GetComponents<EntityComponent>();
        foreach (var comp in gotComponents)
        {
            comp.entity = this;
            components.Add(comp);
            componentsDic.Add(comp.GetType(), comp);
        }
        // sort by order
        components.Sort((a, b) => EntityComponentOrder.GetOrder(a.GetType()).CompareTo(EntityComponentOrder.GetOrder(b.GetType())));
        // add to phase list
        List<PhaseFlag> phaseList = new()
        {
            PhaseFlag.OnEntityAwake,
            PhaseFlag.OnEntityEnable,
            PhaseFlag.OnEntityStart,
            PhaseFlag.PreEntityUpdate,
            PhaseFlag.OnEntityUpdate,
            PhaseFlag.PostEntityUpdate,
            PhaseFlag.PreEntityFixedUpdate,
            PhaseFlag.OnEntityFixedUpdate,
            PhaseFlag.PostEntityFixedUpdate,
            PhaseFlag.PreEntityLateUpdate,
            PhaseFlag.OnEntityLateUpdate,
            PhaseFlag.PostEntityLateUpdate,
            PhaseFlag.OnEntityDisable,
            PhaseFlag.OnEntityDestroy,
        };
        
        foreach (var comp in components)
        {
            foreach (var phase in phaseList)
            {
                if (!phaseComponents.ContainsKey(phase))
                {
                    phaseComponents.Add(phase, new List<EntityComponent>());
                }

                if (comp.PhaseFlag.Contains(phase))
                {
                    phaseComponents[phase].Add(comp);
                }
            }
        }
    }
    
    protected void Awake()
    {
        id = -1;
        EntityManager.Instance.RegisterEntity(this);
        RefreshEntityComponents();
        foreach (var comp in phaseComponents[PhaseFlag.OnEntityAwake])
        {
            comp.OnEntityAwake();
        }
    }

    protected void OnEnable()
    {
        foreach (var comp in phaseComponents[PhaseFlag.OnEntityEnable])
        {
            comp.OnEntityEnable();
        }
    }

    protected void Start()
    {
        foreach (var comp in phaseComponents[PhaseFlag.OnEntityStart])
        {
            comp.OnEntityStart();
        }
    }

    protected void Update()
    {
        foreach (var comp in phaseComponents[PhaseFlag.PreEntityUpdate])
        {
            if (comp.enabled)
                comp.PreEntityUpdate();
        }
        foreach (var comp in phaseComponents[PhaseFlag.OnEntityUpdate])
        {
            if (comp.enabled)
                comp.OnEntityUpdate();
        }
        foreach (var comp in phaseComponents[PhaseFlag.PostEntityUpdate])
        {
            if (comp.enabled)
                comp.PostEntityUpdate();
        }
    }

    protected void FixedUpdate()
    {
        foreach (var comp in phaseComponents[PhaseFlag.PreEntityFixedUpdate])
        {
            if (comp.enabled)
                comp.PreEntityFixedUpdate();
        }
        foreach (var comp in phaseComponents[PhaseFlag.OnEntityFixedUpdate])
        {
            if (comp.enabled)
                comp.OnEntityFixedUpdate();
        }
        foreach (var comp in phaseComponents[PhaseFlag.PostEntityFixedUpdate])
        {
            if (comp.enabled)
                comp.PostEntityFixedUpdate();
        }
    }

    protected void LateUpdate()
    {
        foreach (var comp in phaseComponents[PhaseFlag.PreEntityLateUpdate])
        {
            if (comp.enabled)
                comp.PreEntityLateUpdate();
        }
        foreach (var comp in phaseComponents[PhaseFlag.OnEntityLateUpdate])
        {
            if (comp.enabled)
                comp.OnEntityLateUpdate();
        }
        foreach (var comp in phaseComponents[PhaseFlag.PostEntityLateUpdate])
        {
            if (comp.enabled)
                comp.PostEntityLateUpdate();
        }
    }

    protected void OnDisable()
    {
        foreach (var comp in phaseComponents[PhaseFlag.OnEntityDisable])
        {
            comp.OnEntityDisable();
        }
    }

    // 不采用额外编写的 DestroyEntity 来替代 Object.Destroy,
    // 原本这样做是避免在 OnDestroy 中有未定义的 Instantiate 逻辑(例如场景卸载也会调用Destroy,那么就不应该做逻辑相关的东西)
    // 我们日后采用消息系统调用 Destroy, 而非手动调用, 这样在消息系统中就可以编写删除前的逻辑了

	public static void DestroyEntity(MonoEntity entity) 
	{
		
	}


    protected void OnDestroy()
    {
        foreach (var comp in phaseComponents[PhaseFlag.OnEntityDestroy])
        {
            comp.OnEntityDestroy();
        }
    }
   
}