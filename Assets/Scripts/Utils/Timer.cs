using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


public class Timer
{
    private readonly MonoBehaviour ctx; // nonnull
    private Coroutine timer = null;
    public Timer(MonoBehaviour ctx)
    {
        this.ctx = ctx;
    }

    public void Reset(float time, Action action)
    {
        Stop();
        timer = ctx.StartCoroutine(Func(time, action));
    }

    public void Stop()
    {
        if (timer != null)
        {
           ctx.StopCoroutine(timer);
           timer = null;
        }
    }

    private static IEnumerator Func(float time, Action action)
    {
        yield return new WaitForSeconds(time);
        action();
    }
}
