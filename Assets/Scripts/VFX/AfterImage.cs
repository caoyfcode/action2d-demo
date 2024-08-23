using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class AfterImage : MonoBehaviour
{
    public SpriteRenderer sr;
    public Color color;
    private float aliveTime = 0.2f;
    private float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        timer = aliveTime;
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        color.a = Mathf.Clamp01(0.8f * timer / aliveTime);
        sr.color = color;
        if (timer <= 0)
        {
            Destroy(gameObject);
        }
    }
}
