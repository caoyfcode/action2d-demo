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
        heath.Total = (int)Game.Player.data.maxHeath;
        heath.Value = Game.Player.data.heath;
        magic.Total = (int)Game.Player.data.maxMagic;
        magic.Value = Game.Player.data.magic;
    }
}
