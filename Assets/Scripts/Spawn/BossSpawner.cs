using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpawner : Spawner
{
    public PolygonCollider2D bossArea;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !spawned)
        {
            //spawn boss
            Spawn();
            spawnedObject.GetComponent<BossController>().StartBossFight();
        }
    }
}
