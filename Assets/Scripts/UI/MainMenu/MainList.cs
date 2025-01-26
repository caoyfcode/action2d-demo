using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainList : Menu
{
    public int selectedId = 0; // 0: start, 1: settigns, 2: exit
    public ItemBox start;
    public ItemBox settings;
    public ItemBox exit;

    private ItemBox GetItem(int index) => index switch {
        0 => start,
        1 => settings,
        2 => exit,
        _ => null,
    };

    void Awake()
    {
        selectedId = 0;
        start.selected = true;
    }


    void Update()
    {
        UpdateSelection();
        HandleInput();
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            switch (selectedId)
            {
                case 0:
                    StartGame();
                    break;
                case 1:
                    // open settings
                    break;
                case 2:
                    ExitGame();
                    break;
                default:
                    break;
            }
        }
    }

    void UpdateSelection()
    {
        bool up = Input.GetKeyDown(KeyCode.W);
        bool down = Input.GetKeyDown(KeyCode.S);
        int prevSelected = selectedId;
        if (down && !up)
        {
            if (selectedId < 2)
            {
                selectedId += 1;
            }
            else
            {
                selectedId = 0;
            }
        }
        else if (up && !down)
        {
            if (selectedId > 0)
            {
                selectedId -= 1;
            }
            else
            {
                selectedId = 2;
            }
        }
        if (prevSelected != selectedId)
        {
            if (prevSelected >= 0 && prevSelected < 3)
            {
                GetItem(prevSelected).selected = false;
            }
            if (selectedId >= 0 && selectedId < 3)
            {
                GetItem(selectedId).selected = true;
            }
        }
    }

    void StartGame()
    {
        // TODO 打开存档列表
        DarkScreen.instance.DarkScreenWait(SceneManager.LoadSceneAsync("World"));
        // TODO DataManager 加载数据
    }

    void ExitGame()
    {
        GameManager.ExitGame();
    }
}
