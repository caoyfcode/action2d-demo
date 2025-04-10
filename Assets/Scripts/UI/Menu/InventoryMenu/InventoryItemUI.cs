using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 96x96 的物品图标
public class InventoryItemUI : SelectItem
{
    public Image icon;
    // TODO count ui

    public InventoryItem item;
    public bool hasItem = false;

    // Start is called before the first frame update
    void Start()
    {
        if (hasItem)
        {
            icon.enabled = true;
            icon.sprite = WorldManager.instance.itemData.GetItemSprite(item.id);
        }
        else
        {
            icon.sprite = null;
            icon.enabled = false;
        }
    }

    public void SetItem(InventoryItem item)
    {
        if (item != null)
        {
            this.item = new InventoryItem {
                id = item.id,
                quantity = item.quantity,
            };
            hasItem = true;
            icon.enabled = true;
            icon.sprite = WorldManager.instance.itemData.GetItemSprite(item.id);
        }
        else
        {
            this.item = null;
            hasItem = false;
            icon.sprite = null;
            icon.enabled = false;
        }
    }

    public void UnsetItem()
    {
        SetItem(null);
    }
}
