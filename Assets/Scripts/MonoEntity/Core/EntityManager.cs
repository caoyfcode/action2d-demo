using System.Collections.Generic;

public class EntityManager : SingletonBase<EntityManager>
{
	private int idCounter = 0;
    private List<MonoEntity> allEntity = new();
    private Dictionary<int, MonoEntity> entityDic = new();

    public void RegisterEntity(MonoEntity entity)
    {
        if (entityDic.ContainsKey(entity.id)) 
		{
			return;
		}
		entity.id = idCounter;
		idCounter++;
		allEntity.Add(entity);
    }
}