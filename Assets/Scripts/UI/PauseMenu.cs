using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public bool Shown
    {
        get { return gameObject.activeSelf; }
        set 
        { 
            if (!gameObject.activeSelf && value)
            {
                Game.Pause = true;
            }
            else if (gameObject.activeSelf && !value)
            {
                Game.Pause = false;
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
        // TODO: Open settings menu
    }

    public void Exit()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
