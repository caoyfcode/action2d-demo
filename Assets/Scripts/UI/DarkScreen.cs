using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public IEnumerator WaitForLoading(IEnumerator loadingCoroutine)
    {
        if (!DarkScreenPlaying)
        {
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
