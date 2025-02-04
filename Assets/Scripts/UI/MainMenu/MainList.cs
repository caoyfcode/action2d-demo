using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainList : Menu
{
    public SelectGroup selectGroup;

    public int selectedId = 0; // 0: start, 1: settigns, 2: exit
    public SelectItem start;
    public SelectItem settings;
    public SelectItem exit;

    private SelectItem GetItem(int index) => index switch {
        0 => start,
        1 => settings,
        2 => exit,
        _ => null,
    };

    void Awake()
    {
        selectGroup.SetSelectedInstantly(0); // start
    }

    void Update()
    {
        bool up = Input.GetKeyDown(KeyCode.W);
        bool down = Input.GetKeyDown(KeyCode.S);
        selectGroup.UpdateSelection(down, up);
        if (Input.GetKeyDown(KeyCode.K))
        {
            selectGroup.InvokeAction();
        }
    }

    public void StartGame()
    {
        // TODO 打开存档列表
        DarkScreen.instance.DarkScreenWait(SceneManager.LoadSceneAsync("World"));
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
