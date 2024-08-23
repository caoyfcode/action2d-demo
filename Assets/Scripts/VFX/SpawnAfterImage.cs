using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnAfterImage : MonoBehaviour
{
    public SpriteRenderer source;
    public GameObject afterImagePrefab;
    private float timer = 0;
    private float spawnDuration = 0.02f;
    private bool _spawn = false;
    public bool Spawn
    {
        get => _spawn;
        set
        {
            if (_spawn == value)
                return;
            if (value)
            {
                timer = 0;
            }
            _spawn = value;
        }
    }

    private void Update()
    {
        if (!_spawn)
            return;
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            timer += spawnDuration;
            var obj = Instantiate(afterImagePrefab);
            obj.transform.position = transform.position;
            obj.transform.rotation = transform.rotation;
            var afterImage = obj.GetComponent<AfterImage>();
            afterImage.sr.sprite = source.sprite;
        }
    }
}
