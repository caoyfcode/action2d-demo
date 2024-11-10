using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Level : MonoBehaviour
{
    public string areaName = "";
    // Vector2Int levelIndex;
    public GameObject Spawners;
    public Collider2D bound;


    void Awake()
    {
        if (!bound)
            bound = GetComponent<Collider2D>();
    }

    void Start()
    {
        var spawners = GetComponentsInChildren<Spawner>();
        foreach (var spawner in spawners)
        {
            if (spawner is not BossSpawner)
                spawner.Spawn(gameObject);
        }
    }

}
