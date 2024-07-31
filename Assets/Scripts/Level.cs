using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Level : MonoBehaviour
{
    // string AreaName;
    // Vector2Int levelIndex;
    [HideInInspector]
    public Collider2D bounds;

    void Start()
    {
        bounds = GetComponent<Collider2D>();
        Game.SetCurrentLevel(this);
    }
}
