using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public GameObject spawnedObject;
    public bool spawned = false;


    public void Spawn(GameObject parent = null)
    {
        if (!spawned)
        {
            spawnedObject = Instantiate(enemyPrefab, transform.position, transform.rotation);
            spawned = true;
            if (parent != null)
            {
                spawnedObject.transform.parent = parent.transform;
            }
        }
    }

    public void Despawn()
    {
        if (spawned)
        {
            Destroy(spawnedObject);
            spawned = false;
        }
    }

    public void Respawn()
    {
        Despawn();
        Spawn();
    }
}
