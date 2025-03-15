using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupItem : Interactive
{
    public InventoryItem item;

    void Update()
    {
        if (playerIn && !UIManager.MenuShown)
        {
            HintShown = true;
            if (GameInputManager.MoveTrigger.y != 0)
            {
                HintShown = false;
                hint.gameObject.SetActive(false);
                // WorldManager.Player.PickupItem(item);
                // Shown Pick Message
                UIManager.instance.pickupHud.AddItem(item);
                Destroy(gameObject);
            }
        }
        else
        {
            HintShown = false;
        }
    }
}
