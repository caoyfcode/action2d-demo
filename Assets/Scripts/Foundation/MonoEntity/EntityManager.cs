using System.Collections.Generic;
using System.Linq;
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

    #region Tick
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
    #endregion
    
    // 不一定有用
	#region Singleton
    public T GetSingletonComponent<T>() where T : EntityComponent
    {
	    int order = EntityComponentOrder.GetOrder(typeof(T));
	    int length = allComponents[order].Count;
	    if (length == 0)
	    {
		    Debug.LogError($"EntityManager: No singleton component found for type {typeof(T).Name}");
		    return null;
	    }
	    else if (length > 1)
	    {
		    Debug.LogWarning($"EntityManager: Multiple singleton components found for type {typeof(T).Name}");
	    }

	    return (T)allComponents[order].ElementAt(0);
    }
    
    public MonoEntity GetSingletonEntity<T>() where T : EntityComponent
    {
	    var comp = GetSingletonComponent<T>();
	    if (comp == null)
	    {
		    return null;
	    }
	    else
	    {
		    return comp.entity;
	    }
    }
    #endregion
}