using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : Interactive
{
    void Update()
    {
        if (playerIn && !UIManager.MenuShown)
        {
            HintShown = true;
            if (GameInputManager.MoveTrigger.y != 0)
            {
                HintShown = false;
                hint.gameObject.SetActive(false);
                UIManager.CheckpointMenu.Shown = true;
                DataManager.Ins.PlayerData.checkpoint = WorldManager.Ins.currentLevel.id;
                DataManager.Ins.SaveCurrentSlot();
            }
        }
        else
        {
            HintShown = false;
        }
    }
}
