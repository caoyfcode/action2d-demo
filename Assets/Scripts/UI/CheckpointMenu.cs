using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointMenu : MonoBehaviour
{
    public bool Shown
    {
        get { return gameObject.activeSelf; }
        set 
        { 
            if (!gameObject.activeSelf && value)
            {
                Game.Player.stateMachine.ChangeState(Game.Player.menuState);
            }
            else if (gameObject.activeSelf && !value)
            {
                Game.Player.stateMachine.ChangeState(Game.Player.groundFreeState);
            }
            gameObject.SetActive(value); 
        }
    }

    public void Heal()
    {
        Game.Player.data.heath = Game.Player.data.maxHeath;
        Game.Player.data.magic = Game.Player.data.maxMagic;
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
