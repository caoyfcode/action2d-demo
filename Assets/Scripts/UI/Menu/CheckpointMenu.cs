using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointMenu : ShownUI
{
    public SelectGroup selectGroup;

    private void OnEnable()
    {
        var found = DataManager.instance.saveSlot.checkpoints.Find(p => p.id == WorldManager.instance.currentLevel.id);
        if (found != null)
        {
            found.open = true;
        }
        else
        {
            DataManager.instance.saveSlot.checkpoints.Add(new SaveSlotData.CheckpointSaveData()
            {
                id = WorldManager.instance.currentLevel.id,
                open = true,
            });
        }
        DataManager.instance.SaveCurrentSlot();
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
        if (GameInputManager.UiCancel)
        {
            Shown = false;
        }
    }

    public void Heal()
    {
        WorldManager.Player.Heal();
        Shown = false;
        // WorldManager.ReloadCurrentLevel();
        WorldManager.TeleportCheckpoint(WorldManager.instance.currentLevel.id);
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
