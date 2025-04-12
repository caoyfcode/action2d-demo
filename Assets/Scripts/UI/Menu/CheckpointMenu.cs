using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointMenu : ShownUI
{
    public SelectGroup selectGroup;

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

    public void Heal()
    {
        WorldManager.Player.Heal();
        Shown = false;
        WorldManager.ReloadCurrentLevel();
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
