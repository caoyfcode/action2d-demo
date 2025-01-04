using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryMenu : Menu
{
    public GameObject header;
    public ItemBox mapHeader;
    public ItemBox inventoryHeader;
    public ItemBox questHeader;
    public InventoryMap map;
    public Inventory inventory;
    public int selectedMenu = 1; // 0: Map, 1: Inventory, 2: Quest
    

    void OnEnable()
    {
        HandleShownMenu();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && selectedMenu >= 1)
        {
            selectedMenu -= 1;
            HandleShownMenu();
        }
        else if (Input.GetKeyDown(KeyCode.E) && selectedMenu <= 1)
        {
            selectedMenu += 1;
            HandleShownMenu();
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
