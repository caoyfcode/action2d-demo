using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryMenu : ShownUI
{
    public GameObject header;
    public SelectItem mapHeader;
    public SelectItem inventoryHeader;
    public SelectItem questHeader;
    public MapUI map;
    public InventoryUI inventory;
    public int selectedMenu = 1; // 0: Map, 1: Inventory, 2: Quest
    

    void OnEnable()
    {
        HandleShownMenu();
    }

    void Update()
    {
        if (GameInputManager.UiSwitchLeft && selectedMenu >= 1)
        {
            selectedMenu -= 1;
            HandleShownMenu();
            SoundManager.instance.PlayUISelection();
        }
        else if (GameInputManager.UiSwitchRight && selectedMenu <= 1)
        {
            selectedMenu += 1;
            HandleShownMenu();
            SoundManager.instance.PlayUISelection();
        }
        if (Shown && GameInputManager.Inventory)
        {
            Shown = false;
            SoundManager.instance.PlayUIConfirm();
        }
    }

    void HandleShownMenu()
    {
        switch (selectedMenu)
        {
            case 0:
                mapHeader.selected = true;
                inventoryHeader.selected = false;
                questHeader.selected = false;
                map.Shown = true;
                inventory.Shown = false;
                break;
            case 1:
                mapHeader.selected = false;
                inventoryHeader.selected = true;
                questHeader.selected = false;
                map.Shown = false;
                inventory.Shown = true;
                break;
            case 2:
                mapHeader.selected = false;
                inventoryHeader.selected = false;
                questHeader.selected = true;
                map.Shown = false;
                inventory.Shown = false;
                break;
            default:
                break;
        }
    }
}
