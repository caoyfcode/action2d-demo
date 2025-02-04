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
    void Awake()
    {
        if (hasItem)
        {
            icon.sprite = WorldManager.instance.itemData.GetItemData(item.id).icon;
        }
    }

    public void SetItem(InventoryItem item)
    {
        this.item = item;
        if (item != null)
        {
            hasItem = true;
            icon.sprite = WorldManager.instance.itemData.GetItemData(item.id).icon;
        }
    }
}
