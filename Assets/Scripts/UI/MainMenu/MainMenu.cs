using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : SingletonBase<MainMenu>
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
        SoundManager.instance.PlayBackgroundMusic(SoundManager.instance.mainMenuMusic);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && settingsMenu.Shown)
        {
            CloseSettingsMenu();
            SoundManager.instance.PlayUIConfirm();
        }
    }


    void OnDestroy()
    {
        instance = null;
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
