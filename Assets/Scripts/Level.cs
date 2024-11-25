using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public string areaName = "";
    // Vector2Int levelIndex;
    public GameObject Spawners;


    void Start()
    {
        var spawners = GetComponentsInChildren<Spawner>();
        foreach (var spawner in spawners)
        {
            if (spawner is not BossSpawner)
                spawner.Spawn();
        }
    }

}
