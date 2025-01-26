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
