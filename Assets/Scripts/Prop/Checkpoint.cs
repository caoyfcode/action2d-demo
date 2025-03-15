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
                DataManager.instance.PlayerData.checkpoint = WorldManager.instance.currentLevel.levelIndex;
                DataManager.instance.SaveCurrentSlot();
            }
        }
        else
        {
            HintShown = false;
        }
    }
}
