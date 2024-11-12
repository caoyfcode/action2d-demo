using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO 改为单例类而非在 Game 中引用, 因为此类需要在开始场景以及游戏场景中使用，Game只需要在游戏场景中使用
public class DarkScreen : MonoBehaviour
{
    const string ANIM_FADEOUT = "FadeOut";
    public Animator animator;
    public GameObject playerImage;
    public bool isFadeOutEnd;
    public bool isEnd;
    bool IsLoading
    {
        set
        {
            animator.SetBool("IsLoading", value);
        }
    }
    public bool DarkScreenPlaying;

    private void Start()
    {
        isEnd = false;
        isFadeOutEnd = false;
        IsLoading = false;
        DarkScreenPlaying = false;
        playerImage.SetActive(false);
    }

    // TODO 改为在一个队列中添加任务, 在协程中直到任务耗尽，在Update中如果不在加载中并且队列不为空，则开始加载
    public IEnumerator WaitForLoading(IEnumerator loadingCoroutine)
    {
        if (!DarkScreenPlaying)
        {
            DarkScreenPlaying = true;
            isEnd = false;
            animator.Play(ANIM_FADEOUT, -1, 0);
            IsLoading = true;
            yield return new WaitUntil(() => isFadeOutEnd);
            yield return loadingCoroutine;
            IsLoading = false;
            yield return new WaitWhile(() => isEnd);
            DarkScreenPlaying = false;
        }
    }
}
