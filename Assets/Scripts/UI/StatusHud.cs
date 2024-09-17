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
        heath.Total = (int)Game.Player.status.maxHeath;
        heath.value = Game.Player.status.heath;
        magic.Total = (int)Game.Player.status.maxMagic;
        magic.value = Game.Player.status.magic;
    }
}
