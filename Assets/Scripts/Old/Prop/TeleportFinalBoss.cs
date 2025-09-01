using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportFinalBoss : Interactive
{
    private const string finalBossLevel = "014";
    private Vector2 teleportPos = new(60, 84);
    void Update()
    {
        if (playerIn && !UIManager.MenuShown)
        {
            HintShown = true;
            if (GameInputManager.MoveTrigger.y != 0)
            {
                HintShown = false;
                hint.gameObject.SetActive(false);
                WorldManager.Teleport(finalBossLevel, teleportPos);
            }
        }
        else
        {
            HintShown = false;
        }
    }
}
