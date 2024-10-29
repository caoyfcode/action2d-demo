using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnAfterImage : MonoBehaviour
{
    public SpriteRenderer source;
    private float timer = 0;
    public float maxAliveTime = 0.25f;
    public float spawnDuration = 0.04f;
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
                totalTime = 0;
            }
            _spawn = value;
        }
    }
    public float totalTime;

    private void FixedUpdate()
    {
        if (!_spawn)
            return;
        timer -= Time.fixedDeltaTime;
        totalTime += Time.fixedDeltaTime;
        if (timer <= 0)
        {
            timer = spawnDuration;
            var obj = Instantiate(Game.VFXLoader.afterImagePrefab, transform.position, transform.rotation);
            var afterImage = obj.GetComponent<AfterImage>();
            afterImage.sr.sprite = source.sprite;
            afterImage.SetAliveTime(maxAliveTime - totalTime);
        }
    }

    public void SpawnOne()
    {
        var obj = Instantiate(Game.VFXLoader.afterImagePrefab, transform.position, transform.rotation);
        var afterImage = obj.GetComponent<AfterImage>();
        afterImage.sr.sprite = source.sprite;
    }
}
