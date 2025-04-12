using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    // TODO 改为按 id 寻找 prefab, 然后生成
    public GameObject enemyPrefab;
    public GameObject spawnedObject;
    public bool spawned = false;

    public void Spawn()
    {
        if (!spawned)
        {
            spawnedObject = Instantiate(enemyPrefab, transform.position, transform.rotation);
            spawned = true;
            spawnedObject.transform.parent = transform;
        }
    }

    public void Despawn()
    {
        if (spawned)
        {
            if (spawnedObject != null)
                Destroy(spawnedObject);
            spawned = false;
        }
    }

    public void Respawn()
    {
        Despawn();
        Spawn();
    }

    protected virtual void OnDestroy()
    {
        Despawn();
    }
}
