using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainList : ShownUI
{
    public SelectGroup selectGroup;

    void Awake()
    {
        selectGroup.SetSelectedInstantly(0); // start
    }

    void Update()
    {
        // bool up = GameInputManager.MoveTrigger.y > 0;
        // bool down = GameInputManager.MoveTrigger.y < 0;
        // selectGroup.UpdateSelection(down, up);
        selectGroup.UpdateSelection();
        if (GameInputManager.UiConfirm)
        {
            selectGroup.InvokeAction();
        }
    }

    public void StartGame()
    {
        // TODO 打开存档列表
        DarkScreen.instance.DarkScreenWait(SceneManager.LoadSceneAsync("World"));
        SoundManager.instance.StopBackgroundMusic();
        // TODO DataManager 加载数据
    }

    public void OpenSettingsMenu()
    {
        MainMenu.instance.OpenSettingsMenu();
    }

    public void ExitGame()
    {
        GameManager.ExitGame();
    }
}
