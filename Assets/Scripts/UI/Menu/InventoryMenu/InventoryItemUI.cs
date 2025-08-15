using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 96x96 的物品图标
public class InventoryItemUI : SelectItem
{
    public Image icon;
    // TODO count ui

    // nonnull
    public InventoryItem item = new() {
        id = "",
        quantity = 0,
    };
    public bool hasItem = false;

    // Start is called before the first frame update
    void Start()
    {
        if (hasItem)
        {
            SetItem(item);
        }
        else
        {
            UnsetItem();
        }
    }

    public void SetItem(InventoryItem item)
    {
        var sprite = WorldManager.Ins.itemData.GetItemSprite(item.id);
        if (sprite == null)
        {
            UnsetItem();
            return;
        }
        this.item.id = item.id;
        this.item.quantity = item.quantity;
        hasItem = true;
        icon.enabled = true;
        icon.sprite = sprite;
    }

    public void UnsetItem()
    {
        item.id = "";
        item.quantity = 0;
        hasItem = false;
        icon.sprite = null;
        icon.enabled = false;
    }
}
