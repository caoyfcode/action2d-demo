using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class AfterImage : MonoBehaviour
{
    public SpriteRenderer sr;
    private float timer = 0;
    private float aliveTime = 0f;

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        Color color = sr.color;
        color.a = Mathf.Clamp01(timer / aliveTime);
        sr.color = color;
        if (timer <= 0)
        {
            Destroy(gameObject);
        }
    }

   // 需要在第一次 Update 之前调用
    public void SetAliveTime(float aliveTime)
    {
        timer = aliveTime;
        this.aliveTime = aliveTime;
    }
}
