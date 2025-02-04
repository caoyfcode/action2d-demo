using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointMenu : Menu
{
    public SelectGroup selectGroup;

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

    public void Heal()
    {
        WorldManager.Player.Heal();
        Shown = false;
    }

    public void Teleport()
    {
        Shown = false;
        UIManager.TeleportMenu.Shown = true;
    }

    public void Upgrade()
    {
        // nothing
    }

    public void Leave()
    {
        Shown = false;
    }

}
