using System.Collections.Generic;
using UnityEngine;

public class EntityManager : SingletonBase<EntityManager>
{
	private int idCounter = 0;
    private Dictionary<int, MonoEntity> entityDic = new();
    private HashSet<EntityComponent>[] allComponents = new HashSet<EntityComponent>[EntityComponentOrder.TypeCount];

    public void Init()
    {
	    idCounter = 0;
	    entityDic = new();
	    allComponents = new HashSet<EntityComponent>[EntityComponentOrder.TypeCount];
	    for (int i = 0; i < EntityComponentOrder.TypeCount; i++)
	    {
		    allComponents[i] = new ();
	    }
    }
    
    public void RegisterEntity(MonoEntity entity)
    {
        if (entityDic.ContainsKey(entity.id)) 
		{
			return;
		}
		entity.id = idCounter;
		idCounter++;
		entityDic.Add(entity.id, entity);
		foreach (var comp in entity.Components)
		{
			int order = EntityComponentOrder.GetOrder(comp.GetType());
			allComponents[order].Add(comp);
		}
    }

    public void UnregisterEntity(MonoEntity entity)
    {
	    if (!entityDic.ContainsKey(entity.id))
	    {
		    return;
	    }
	    
	    entityDic.Remove(entity.id);
	    foreach (var comp in entity.Components)
	    {
		    int order = EntityComponentOrder.GetOrder(comp.GetType());
		    allComponents[order].Remove(comp);
	    }
    }

    public bool IsValid(MonoEntity entity)
    {
	    return entity != null && entity.id >= 0 && entityDic.ContainsKey(entity.id);
    }

    public void DestroyEntity(MonoEntity entity, bool immediate = false)
    {
	    // TODO
	    if (!IsValid(entity))
	    {
		    return;
	    }
		entity.PreEntityDestroy();
		if (!immediate)
		{
			Object.Destroy(entity.gameObject);
		}
		else
		{
			Object.DestroyImmediate(entity.gameObject);
		}
    }

    // TODO 这个 update 何时调用?
    public void UpdateAll()
    {
	    foreach (var comps in allComponents)
	    {
		    foreach (var comp in comps)
		    {
			    if (!comp.PhaseFlag.Contains(PhaseFlag.PreEntityUpdate))
			    {
				    break;
			    }
			    if (comp.entity.enabled && comp.entity.Started)
			    {
				    comp.PreEntityUpdate();
			    }
		    }
	    }
	    
	    foreach (var comps in allComponents)
	    {
		    foreach (var comp in comps)
		    {
			    if (!comp.PhaseFlag.Contains(PhaseFlag.OnEntityUpdate))
			    {
				    break;
			    }
			    if (comp.entity.enabled && comp.entity.Started)
			    {
				    comp.OnEntityUpdate();
			    }
		    }
	    }
	    
	    foreach (var comps in allComponents)
	    {
		    foreach (var comp in comps)
		    {
			    if (!comp.PhaseFlag.Contains(PhaseFlag.PostEntityUpdate))
			    {
				    break;
			    }
			    if (comp.entity.enabled && comp.entity.Started)
			    {
				    comp.PostEntityUpdate();
			    }
		    }
	    }
	    
	    // // update every entity
	    // // TODO 下面只分了阶段, 没有将不同实体的组件顺序分开，未来可以考虑
	    // // TODO 或者未来不考虑分开，只用阶段作为不同entity的tick排序, 用组件顺序作为entity内部tick顺序
	    // foreach (var entity in allEntity)
	    // {
		   //  if (!entity.enabled || !entity.Started)
		   //  {
			  //   continue;
		   //  }
		   //  foreach (var comp in entity.phaseComponents[PhaseFlag.PreEntityUpdate])
		   //  {
			  //   // 不关心 comp 本身的 enabled
			  //   comp.PreEntityUpdate();
		   //  }
	    // }
	    // foreach (var entity in allEntity)
	    // {
		   //  if (!entity.enabled || !entity.Started)
		   //  {
			  //   continue;
		   //  }
		   //  foreach (var comp in entity.phaseComponents[PhaseFlag.OnEntityUpdate])
		   //  {
			  //   // 不关心 comp 本身的 enabled
			  //   comp.OnEntityUpdate();
		   //  }
	    // }
	    // foreach (var entity in allEntity)
	    // {
		   //  if (!entity.enabled || !entity.Started)
		   //  {
			  //   continue;
		   //  }
		   //  foreach (var comp in entity.phaseComponents[PhaseFlag.PostEntityUpdate])
		   //  {
			  //   // 不关心 comp 本身的 enabled
			  //   comp.PostEntityUpdate();
		   //  }
	    // }
    }

    public void FixedUpdateAll()
    {
	    foreach (var comps in allComponents)
	    {
		    foreach (var comp in comps)
		    {
			    if (!comp.PhaseFlag.Contains(PhaseFlag.PreEntityFixedUpdate))
			    {
				    break;
			    }
			    if (comp.entity.enabled && comp.entity.Started)
			    {
				    comp.PreEntityFixedUpdate();
			    }
		    }
	    }
	    
	    foreach (var comps in allComponents)
	    {
		    foreach (var comp in comps)
		    {
			    if (!comp.PhaseFlag.Contains(PhaseFlag.OnEntityFixedUpdate))
			    {
				    break;
			    }
			    if (comp.entity.enabled && comp.entity.Started)
			    {
				    comp.OnEntityFixedUpdate();
			    }
		    }
	    }
	    
	    foreach (var comps in allComponents)
	    {
		    foreach (var comp in comps)
		    {
			    if (!comp.PhaseFlag.Contains(PhaseFlag.PostEntityFixedUpdate))
			    {
				    break;
			    }
			    if (comp.entity.enabled && comp.entity.Started)
			    {
				    comp.PostEntityFixedUpdate();
			    }
		    }
	    }
    }

    public void LateUpdateAll()
    {
	    foreach (var comps in allComponents)
	    {
		    foreach (var comp in comps)
		    {
			    if (!comp.PhaseFlag.Contains(PhaseFlag.PreEntityLateUpdate))
			    {
				    break;
			    }
			    if (comp.entity.enabled && comp.entity.Started)
			    {
				    comp.PreEntityLateUpdate();
			    }
		    }
	    }
	    
	    foreach (var comps in allComponents)
	    {
		    foreach (var comp in comps)
		    {
			    if (!comp.PhaseFlag.Contains(PhaseFlag.OnEntityLateUpdate))
			    {
				    break;
			    }
			    if (comp.entity.enabled && comp.entity.Started)
			    {
				    comp.OnEntityLateUpdate();
			    }
		    }
	    }
	    
	    foreach (var comps in allComponents)
	    {
		    foreach (var comp in comps)
		    {
			    if (!comp.PhaseFlag.Contains(PhaseFlag.PostEntityLateUpdate))
			    {
				    break;
			    }
			    if (comp.entity.enabled && comp.entity.Started)
			    {
				    comp.PostEntityLateUpdate();
			    }
		    }
	    }
    }


    // 下面的实现是错的, 因为 T 如果是 EntityComponent 子类, 就已经不可能是 MonoSingletonBase<T> 子类了
    // public T GetSingletonComponent<T>() where T : MonoSingletonBase<T>
    // {
	   //  var inst = MonoSingletonBase<T>.instance;
	   //  if (inst == null)
	   //  {
		  //   return null;
	   //  }
	   //  return inst;
    // }
    //
    // public MonoEntity GetSingletonEntity<T>() where T : MonoSingletonBase<T>
    // {
	   //  var inst = GetSingletonComponent<T>();
	   //  if (inst == null)
	   //  {
		  //   return null;
	   //  }
    //
	   //  return inst.GetComponent<MonoEntity>();
    // }
}