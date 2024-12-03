using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointMenu : Menu
{

    public void Heal()
    {
        Game.Player.Heal();
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
