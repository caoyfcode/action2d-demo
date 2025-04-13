using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpawner : Spawner
{
    // boss id 
    // TODO 未来在 Spawner 处按照 id 来生成，而不是直接按照 prefab 来生成
    // 并且 spawner 生成的物体有几种类型
    // 1. boss 按照存档决定是否生成，并且生成有条件
    // 2. 其它永久不刷新的怪物(需要存档)
    // 3. 在休息前不刷新的怪物(不需要存档，但是需要有运行时的数据)
    // 4. 按某种条件生成的怪物(譬如某个点定时刷新)
    // 对于切换Level就刷新的怪物，直接手动放置Level预制件里面
    // TODO 将 ProCamera 的 Room 切换改为手动， 并根据boss是否已经打败来决定是否切换
    public string entityId; 
    public Collider2D roomCollider;
    public Rect room;

    void Awake()
    {
        if (roomCollider)
        {
            room = new Rect(
                roomCollider.bounds.min.x,
                roomCollider.bounds.min.y,
                roomCollider.bounds.size.x,
                roomCollider.bounds.size.y
            );
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !spawned)
        {
            var found = DataManager.instance.GetBossData(entityId);
            if (found == null)
            {
                DataManager.instance.SaveBossData(entityId, false);
            }
            else
            {
                if (found.dead)
                    return;
            }
            //spawn boss
            Spawn();
            var entity = spawnedObject.GetComponent<Entity>();
            entity.onDeath.AddListener(() => DataManager.instance.SaveBossData(entityId, true)); // 当死亡时保存数据
            // TODO 按照具体 boss id 获取组件传递参数
            WorldManager.instance.bossSystem.StartBossFight(
                entity,
                room
            );
        }
    }
}
