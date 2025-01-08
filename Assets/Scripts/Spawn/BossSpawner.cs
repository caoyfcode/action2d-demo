using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpawner : Spawner
{
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
            //spawn boss
            Spawn();
            WorldManager.instance.bossSystem.StartBossFight(
                spawnedObject.GetComponent<Entity>(),
                room
            );
        }
    }
}
