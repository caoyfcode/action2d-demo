using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
                UIManager.instance.pickupHud.AddItem(new InventoryItem() {
                    id = item.id,
                    quantity = item.quantity,
                });
                WorldManager.instance.InventoryAddItem(new InventoryItem() {
                    id = item.id,
                    quantity = item.quantity,
                });
                Destroy(gameObject);
            }
        }
        else
        {
            HintShown = false;
        }
    }
}
