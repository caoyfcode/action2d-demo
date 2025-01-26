using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkScreen : SingletonBase<DarkScreen>
{
    const string ANIM_FADEOUT = "FadeOut";
    public Animator animator;
    public GameObject playerImage;
    public bool isFadeOutEnd;
    public bool isEnd; // 这个变量暂时没有用
    bool Anim_IsLoading
    {
        set
        {
            animator.SetBool("IsLoading", value);
        }
    }
    public bool DarkScreenPlaying;
    private Queue<object> waitQueue = new();


    protected override void Awake()
    {
        base.Awake();
        isEnd = false;
        isFadeOutEnd = false;
        Anim_IsLoading = false;
        DarkScreenPlaying = false;
        playerImage.SetActive(false);
    }

    // void Update()
    // {
    //     // 若仅仅依靠 DarkScreen 中的代码启动，则在 watQueue 的最后一个中若调用了 DarkScreen
    //     // 则不会继续进入队列了，
    //     if (!DarkScreenPlaying && waitQueue.Count > 0)
    //     {
    //         DarkScreenPlaying = true;
    //         animator.Play(ANIM_FADEOUT, -1, 0);
    //         Anim_IsLoading = true;
    //         StartCoroutine(WaitCoroutine());
    //     }
    // }

    public void DarkScreenWait(IEnumerator wait)
    {
        waitQueue.Enqueue(wait);
        if (!DarkScreenPlaying && waitQueue.Count == 1)
        {
            DarkScreenPlaying = true;
            animator.Play(ANIM_FADEOUT, -1, 0);
            Anim_IsLoading = true;
            StartCoroutine(WaitCoroutine());
        }
    }

    public void DarkScreenWait(Action action)
    {
        waitQueue.Enqueue(ActionCoroutine(action));
        if (!DarkScreenPlaying && waitQueue.Count == 1)
        {
            DarkScreenPlaying = true;
            animator.Play(ANIM_FADEOUT, -1, 0);
            Anim_IsLoading = true;
            StartCoroutine(WaitCoroutine());
        }
    }

    public void DarkScreenWait(AsyncOperation operation)
    {
        waitQueue.Enqueue(operation);
        if (!DarkScreenPlaying && waitQueue.Count == 1)
        {
            DarkScreenPlaying = true;
            animator.Play(ANIM_FADEOUT, -1, 0);
            Anim_IsLoading = true;
            StartCoroutine(WaitCoroutine());
        }
    }

    IEnumerator WaitCoroutine()
    {
        // DarkScreenPlaying = true;
        // animator.Play(ANIM_FADEOUT, -1, 0);
        // Anim_IsLoading = true;
        yield return new WaitUntil(() => isFadeOutEnd);
        while (waitQueue.Count > 0)
        {
            yield return waitQueue.Dequeue();
        }
        Anim_IsLoading = false;
        DarkScreenPlaying = false; // 加载结束继续播放 FadeIn 动画，但是不等待结束
    }

    static IEnumerator ActionCoroutine(Action action)
    {
        action();
        yield break;
    }
}
