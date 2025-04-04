using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : ShownUI
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

    public SelectGroup selectGroup;

    void Update()
    {
        bool up = GameInputManager.MoveTrigger.y > 0;
        bool down = GameInputManager.MoveTrigger.y < 0;
        selectGroup.UpdateSelection(down, up);
        if (GameInputManager.UiConfirm)
        {
            selectGroup.InvokeAction();
        }
        if (Shown && (GameInputManager.UiCancel || GameInputManager.Pause))
        {
            Shown = false;
            SoundManager.instance.PlayUIConfirm();
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
        SoundManager.instance.StopBackgroundMusic();
    }
}
