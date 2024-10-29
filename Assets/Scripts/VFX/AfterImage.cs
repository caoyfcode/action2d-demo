using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class AfterImage : MonoBehaviour
{
    public SpriteRenderer sr;
    public Color color;
    private float timer = 0;

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        // color.a = Mathf.Clamp01(0.8f * timer / aliveTime);
        // sr.color = color;
        if (timer <= 0)
        {
            Destroy(gameObject);
        }
    }

   // 需要在第一次 Update 之前调用
    public void SetAliveTime(float aliveTime)
    {
        timer = aliveTime;
    }
}
