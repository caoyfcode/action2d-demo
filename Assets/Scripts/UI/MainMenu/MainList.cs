using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainList : Menu
{
    public SelectGroup selectGroup;

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
