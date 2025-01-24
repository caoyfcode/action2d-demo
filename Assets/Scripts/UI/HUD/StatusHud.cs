using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusHud : MonoBehaviour
{
    public StatusIconList heath;
    public StatusIconList magic;

    // Update is called once per frame
    void Update()
    {
        if (WorldManager.Player == null)
            return;
        heath.Total = (int)WorldManager.Player.info.maxHealth;
        heath.Value = WorldManager.Player.info.health;
        magic.Total = (int)WorldManager.Player.info.maxMagic;
        magic.Value = WorldManager.Player.info.magic;
    }
}
