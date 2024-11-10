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
    public PolygonCollider2D bound;


    void Awake()
    {
        if (!bound)
            bound = GetComponent<PolygonCollider2D>();
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
