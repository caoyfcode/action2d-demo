using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : SingletonBase<SoundManager>
{

    public AudioClip uiSelectionClip;
    public AudioClip uiConfirmClip;

    public AudioSource backgroundMusic; // background music
    public AudioSource uiSoundEffect; // ui and interactive ui
    // environment sound effect
    // attack/hit sound effect
    // baclground music

    
    public void PlayUISoundEffect(AudioClip clip)
    {
        uiSoundEffect.clip = clip;
        uiSoundEffect.Play();
    }

    public void PlayUISelection()
    {
        PlayUISoundEffect(uiSelectionClip);
    }

    // TODO 添加 Confirm 与 Cancel 两种声音
    public void PlayUIConfirm()
    {
        PlayUISoundEffect(uiConfirmClip);
    }

    public void PlayBackgroundMusic(AudioClip clip)
    {
        backgroundMusic.clip = clip;
        backgroundMusic.Play();
    }
}
