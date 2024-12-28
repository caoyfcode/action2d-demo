using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnAfterImage : MonoBehaviour
{
    public SpriteRenderer source;
    private float timer = 0;
    public float maxAliveTime = 0.2f;
    public float spawnDuration = 0.015f;
    public AliveTimeType aliveTimeType = AliveTimeType.FirstMaxTime;
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
    public enum AliveTimeType
    {
        FirstMaxTime,
        AllMaxTime,
    }

    private void Update()
    {
        if (!_spawn)
            return;
        timer -= Time.deltaTime;
        totalTime += Time.deltaTime;
        if (timer <= 0)
        {
            timer = spawnDuration;
            var obj = Instantiate(Game.VFXLoader.afterImagePrefab, transform.position, transform.rotation);
            var afterImage = obj.GetComponent<AfterImage>();
            afterImage.sr.sprite = source.sprite;
            afterImage.sr.renderingLayerMask = source.renderingLayerMask;
            if (aliveTimeType == AliveTimeType.FirstMaxTime)
                afterImage.SetAliveTime(maxAliveTime - totalTime);
            else
                afterImage.SetAliveTime(maxAliveTime);
        }
    }
}
