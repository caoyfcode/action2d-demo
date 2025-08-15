using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoSingletonBase<MainMenu>
{
    public MainList mainList;
    public SettingsMenu settingsMenu;

    public GameObject title;
    public GameObject hint;


    protected override void Awake()
    {
        base.Awake();
        mainList.Shown = true;
        title.SetActive(true);
        hint.SetActive(true);
        settingsMenu.Shown = false;
    }

    void Start()
    {
        SoundManager.Ins.PlayBackgroundMusic(SoundManager.Ins.mainMenuMusic);
    }

    void Update()
    {
        if (GameInputManager.UiCancel && settingsMenu.Shown)
        {
            CloseSettingsMenu();
            DataManager.Ins.SaveSettings();
            SoundManager.Ins.PlayUIConfirm();
        }
    }


    void OnDestroy()
    {
        Ins = null;
    }

    public void OpenSettingsMenu()
    {
        mainList.Shown = false;
        title.SetActive(false);
        hint.SetActive(false);
        settingsMenu.Shown = true;
    }

    public void CloseSettingsMenu()
    {
        mainList.Shown = true;
        title.SetActive(true);
        hint.SetActive(true);
        settingsMenu.Shown = false;
    }
}
