using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : Menu
{
    public override bool Shown
    {
        get { return gameObject.activeSelf; }
        set 
        { 
            if (!gameObject.activeSelf && value)
            {
                GameManager.Pause = true;
            }
            else if (gameObject.activeSelf && !value)
            {
                GameManager.Pause = false;
            }
            gameObject.SetActive(value); 
        }
    }

    public ItemBox resume;
    public ItemBox settings;
    public ItemBox exit;
    private ItemBox GetItem(int index) => index switch {
        0 => resume,
        1 => settings,
        2 => exit,
        _ => null
    };
    public int selected = -1;

    void Update()
    {
        UpdateSelection();
        HandleInput();
    }

    void UpdateSelection()
    {
        bool up = Input.GetKeyDown(KeyCode.W);
        bool down = Input.GetKeyDown(KeyCode.S);
        int prevSelected = selected;
        if (down && !up)
        {
            if (selected < 2)
            {
                selected++;
            }
            else
            {
                selected = 0;
            }
        }
        else if (up && !down)
        {
            if (selected > 0)
            {
                selected--;
            }
            else
            {
                selected = 2;
            }
        }
        if (prevSelected != selected)
        {
            if (prevSelected >= 0 && prevSelected < 3)
            {
                GetItem(prevSelected).selected = false;
            }
            if (selected >= 0 && selected < 3)
            {
                GetItem(selected).selected = true;
            }
        }
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            switch (selected)
            {
                case 0:
                    Resume();
                    break;
                case 1:
                    Settings();
                    break;
                case 2:
                    Exit();
                    break;
            }
        }
    }

    public void Resume()
    {
        Shown = false;
    }

    public void Settings()
    {
        Shown = false;
        UIManager.instance.settingsMenu.Shown = true;
    }

    public void Exit()
    {
        DarkScreen.instance.DarkScreenWait(SceneManager.LoadSceneAsync("MainMenu"));
    }
}
