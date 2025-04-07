using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : ShownUI
{
    public enum SelectedTable
    {
        None,
        AttackConfig,
        Attack,
        Item,
    }
    public enum State
    {
        SelectNone,
        SelectAttackConfig,
        SelectAttackTable,
        SelectItemTable,
        SelectAttackConfigForAttackTable,
        SelectAttactTableForAttackConfig,
    }

    public GameObject attackConfigRoot;
    public GameObject attackTableRoot;
    public GameObject itemTableRoot;
    public SelectGroup attackConfig;
    public SelectGroup attackTable;
    public SelectGroup itemTable;
    public Text descriptionName;
    public Text descriptionContent;
    public State state = State.SelectNone;

    public GameObject itemPrefab;

    public bool debugDescriptionShowId = false; 

    // public SelectedTable selectedTable = SelectedTable.None;
    // public int selectedId = -1;

    void Start()
    {
        // TODO 从角色背包数据生成子结点
        RefreshAttackConfig();
        RefreshAttackTable();
        RefreshItemTable();
    }

    void Update()
    {
        UpdateSelection();
        UpdateDescription();
        HandleInput();
    }

    void HandleInput()
    {
        if (UIManager.instance.inventoryMenu.Shown && GameInputManager.UiCancel)
        {
            UIManager.instance.inventoryMenu.Shown = false;
            SoundManager.instance.PlayUIConfirm();
        }
    }

    void UpdateDescription()
    {
        InventoryItemUI itemUI = null;
        switch (state) {
            case State.SelectAttackConfig:
            case State.SelectAttackConfigForAttackTable:
                if (attackConfig.selected >= 0 && attackConfig.selected < attackConfigRoot.transform.childCount)
                {
                    itemUI = attackConfigRoot.transform.GetChild(attackConfig.selected).GetComponent<InventoryItemUI>();
                }
                break;
            case State.SelectAttackTable:
            case State.SelectAttactTableForAttackConfig:
                if (attackTable.selected >= 0 && attackTable.selected < attackTableRoot.transform.childCount)
                {
                    itemUI = attackTableRoot.transform.GetChild(attackTable.selected).GetComponent<InventoryItemUI>();
                }
                break;
            case State.SelectItemTable:
                if (itemTable.selected >= 0 && itemTable.selected < itemTableRoot.transform.childCount)
                {
                    itemUI = itemTableRoot.transform.GetChild(itemTable.selected).GetComponent<InventoryItemUI>();
                }
                break;
        }
        if (itemUI != null && itemUI.hasItem)
        {
            var itemData = WorldManager.instance.itemData.GetItemData(itemUI.item.id);
            if (itemData != null)
            {
                descriptionName.text = itemData.name;
                if (!debugDescriptionShowId)
                {
                    descriptionContent.text = itemData.description;
                }
                else
                {
                    descriptionContent.text = itemData.description + "\n{" + itemUI.item.id + "}";
                }
                return;
            }
        }
        descriptionName.text = "空";
        descriptionContent.text = "空";
    }

    void UpdateSelection()
    {
        bool left = GameInputManager.MoveTrigger.x < 0;
        bool right = GameInputManager.MoveTrigger.x > 0;
        switch (state)
        {
            case State.SelectNone:
                if (GameInputManager.MoveTrigger != Vector2.zero)
                {
                    state = State.SelectAttackConfig;
                    attackConfig.Select(0);
                }
                break;
            case State.SelectAttackConfig:
                if (right && attackConfig.IsRightMost)
                {
                    if (attackTable.itemActions.Count > 0)
                    {
                        state = State.SelectAttackTable;
                        attackConfig.Unselect();
                        attackTable.Select(0);
                    }
                    else if (itemTable.itemActions.Count > 0)
                    {
                        state = State.SelectItemTable;
                        attackConfig.Unselect();
                        itemTable.Select(0);
                    }
                    // else do nothing
                }
                else
                {
                    attackConfig.UpdateSelection();
                }
                break;
            case State.SelectAttackTable:
                if (left && attackTable.IsLeftMost)
                {
                    state = State.SelectAttackConfig;
                    attackTable.Unselect();
                    attackConfig.Select(attackConfig.GridXMax);
                }
                else if (right && attackTable.IsRightMost)
                {
                    if (itemTable.itemActions.Count > 0)
                    {
                        state = State.SelectItemTable;
                        itemTable.Select(0, attackTable.GridY); // TODO 考虑 scroll
                        attackTable.Unselect();
                    }
                    // else do nothing
                }
                else
                {
                    attackTable.UpdateSelection();
                }
                break;
            case State.SelectItemTable:
                if (left && itemTable.IsLeftMost)
                {
                    if (attackTable.itemActions.Count > 0)
                    {
                        state = State.SelectAttackTable;
                        attackTable.Select(attackTable.GridXMax, itemTable.GridY); // TODO 考虑 scroll
                        itemTable.Unselect();
                    }
                    else
                    {
                        state = State.SelectAttackConfig;
                        attackConfig.Select(attackConfig.GridXMax);
                        itemTable.Unselect();
                    }
                }
                else
                {
                    itemTable.UpdateSelection();
                }
                break;
            case State.SelectAttackConfigForAttackTable:
                attackConfig.UpdateSelection();
                break;
            case State.SelectAttactTableForAttackConfig:
                attackTable.UpdateSelection();
                break;
        }
    }

    public void RefreshAttackConfig()
    {
        var attack1 = attackConfigRoot.transform.GetChild(0).GetComponent<InventoryItemUI>();
        var attack2 = attackConfigRoot.transform.GetChild(1).GetComponent<InventoryItemUI>();
        var attack3 = attackConfigRoot.transform.GetChild(2).GetComponent<InventoryItemUI>();
        // TODO 从角色状态获取三个子结点内容
        attackConfig.RefreshItemsFrom(attackConfigRoot.transform); // 固定 3 个, 每次修改攻击就刷新
    }

    public void RefreshAttackTable()
    {
        // TODO 从角色身上获取已拥有攻击能力，并转化为物品列表
        int childCount = attackTableRoot.transform.childCount;
        // for i < childCount, Instantiate
        // for i > childCount, Destroy
        attackTable.RefreshItemsFrom(attackTableRoot.transform); // 每次技能更新就刷新
    }

    public void RefreshItemTable()
    {
        int childCount = itemTableRoot.transform.childCount;
        var inventory = DataManager.instance.PlayerData.inventory;
        // TODO 构造新的列表，包含角色的被动能力与 inventory
        for (int i = 0; i < inventory.Count && i < childCount; i++)
        {
            var itemObj = itemTableRoot.transform.GetChild(i).gameObject;
            itemObj.GetComponent<InventoryItemUI>().SetItem(inventory[i]);
        }
        if (inventory.Count > childCount)
        {
            for (int i = childCount; i < inventory.Count; i++)
            {
                var itemObj = Instantiate(itemPrefab, itemTableRoot.transform);
                itemObj.GetComponent<InventoryItemUI>().SetItem(inventory[i]);
            }
        }
        else if (inventory.Count < childCount)
        {
            for (int i = childCount - 1; i >= inventory.Count; i++)
            {
                DestroyImmediate(itemTableRoot.transform.GetChild(i).gameObject);
            }
        }
        itemTable.RefreshItemsFrom(itemTableRoot.transform); // 每次拾取物品就刷新
    }


    // void UpdateSelection()
    // {
    //     bool left = GameInputManager.MoveTrigger.x < 0;
    //     bool right = GameInputManager.MoveTrigger.x > 0;
    //     bool up = GameInputManager.MoveTrigger.y > 0;
    //     bool down = GameInputManager.MoveTrigger.y < 0;
        
    //     // 处理前一帧不可用的选中
    //     if (selectedTable != SelectedTable.None)
    //     {
    //         GameObject table = selectedTable switch 
    //         {
    //             SelectedTable.Attack => attackTable,
    //             SelectedTable.Item => itemTable,
    //             _ => attackConfig,
    //         };
    //         if (selectedId < 0 || selectedId >= table.transform.childCount)
    //         {
    //             selectedTable = SelectedTable.None;
    //             selectedId = -1;
    //         }
    //     }

    //     // 计算新的选中
    //     int prevSelected = selectedId;
    //     SelectedTable prevSelectedTable = selectedTable;
    //     if (prevSelectedTable == SelectedTable.None) // 无旧选项
    //     {
    //         if (left || right || up || down)
    //         {
    //             selectedTable = SelectedTable.AttackConfig;
    //             selectedId = 0;
    //         }
    //     }
    //     else if ((up && !down) || (down && !up))  // 旧选项存在，且上下运动
    //     {
    //         if (prevSelectedTable != SelectedTable.AttackConfig)
    //         {
    //             GameObject table = prevSelectedTable == SelectedTable.Attack ? attackTable : itemTable;
    //             if (up && prevSelected >= 4)
    //             {
    //                 selectedId -= 4;
    //             }
    //             else if (down)
    //             {
    //                 if (prevSelected + 4 < table.transform.childCount)
    //                     selectedId += 4;
    //                 else if ((table.transform.childCount - 1) / 4 > prevSelected / 4)
    //                     selectedId = table.transform.childCount - 1;
    //             }
    //         }
    //     }
    //     else if ((left && !right) || (right && !left)) // 旧选项存在，且左右运动
    //     {
    //         if (prevSelectedTable  == SelectedTable.AttackConfig)
    //         {
    //             if (left && prevSelected > 0)
    //             {
    //                 selectedId--;
    //             }
    //             else if (right)
    //             {
    //                 if (prevSelected < attackConfig.transform.childCount - 1)
    //                 {
    //                     selectedId++;
    //                 }
    //                 else if (attackTable.transform.childCount > 0)
    //                 {
    //                     selectedTable = SelectedTable.Attack;
    //                     selectedId = 0;
    //                 }
    //                 else if (itemTable.transform.childCount > 0)
    //                 {
    //                     selectedTable = SelectedTable.Item;
    //                     selectedId = 0;
    //                 }
    //             }
    //         }
    //         else if (prevSelectedTable  == SelectedTable.Attack)
    //         {
    //             int div = prevSelected / 4;
    //             int mod = prevSelected % 4;
    //             if (left)
    //             {
    //                 if (mod == 0)
    //                 {
    //                     selectedTable = SelectedTable.AttackConfig;
    //                     selectedId = attackConfig.transform.childCount - 1;
    //                 }
    //                 else
    //                 {
    //                     mod--;
    //                     selectedId = div * 4 + mod;
    //                 }
    //             }
    //             else // right
    //             {
    //                 if (mod == 3 || prevSelected == attackTable.transform.childCount - 1)
    //                 {
    //                     if (itemTable.transform.childCount > 0)
    //                     {
    //                         selectedTable = SelectedTable.Item;
    //                         selectedId = Mathf.Min(4 * div, (itemTable.transform.childCount - 1) / 4 * 4);
    //                     }
    //                 }
    //                 else
    //                 {
    //                     mod++;
    //                     selectedId = div * 4 + mod;
    //                 }
    //             }
    //         }
    //         else // item table
    //         {
    //             int div = prevSelected / 4;
    //             int mod = prevSelected % 4;
    //             if (right)
    //             {
    //                 if (mod == 3 || prevSelected == itemTable.transform.childCount - 1)
    //                 {
    //                     // nothing
    //                 }
    //                 else
    //                 {
    //                     mod++;
    //                     selectedId = div * 4 + mod;
    //                 }
    //             }
    //             else // left
    //             {
    //                 if (mod > 0)
    //                 {
    //                     mod--;
    //                     selectedId = div * 4 + mod;
    //                 }
    //                 else if (attackTable.transform.childCount > 0)
    //                 {
    //                     selectedTable = SelectedTable.Attack;
    //                     selectedId = Mathf.Min(4 * div + 3, attackTable.transform.childCount - 1);
    //                 }
    //                 else if (attackConfig.transform.childCount > 0)
    //                 {
    //                     selectedTable = SelectedTable.AttackConfig;
    //                     selectedId = attackConfig.transform.childCount - 1;
    //                 }
    //             }
    //         }
    //     }
    //     // 旧选项存在且与新选项不同时，取消旧选项
    //     if (prevSelectedTable != SelectedTable.None &&
    //         (prevSelectedTable != selectedTable || prevSelected != selectedId))
    //     {
    //         GameObject pervTable = prevSelectedTable switch 
    //         {
    //             SelectedTable.Attack => attackTable,
    //             SelectedTable.Item => itemTable,
    //             _ => attackConfig,
    //         };
    //         pervTable.transform.GetChild(prevSelected).GetComponent<SelectItem>().selected = false;
    //     }
    //     // 新选项存在且与旧选项不同时，选中新选项
    //     if (selectedTable != SelectedTable.None &&
    //         (prevSelectedTable != selectedTable || prevSelected != selectedId))
    //     {
    //         GameObject table = selectedTable switch
    //         {
    //             SelectedTable.Attack => attackTable,
    //             SelectedTable.Item => itemTable,
    //             _ => attackConfig,
    //         };
    //         table.transform.GetChild(selectedId).GetComponent<SelectItem>().selected = true;
    //         ChangeDescription();
    //         SoundManager.instance.PlayUISelection();
    //     }
    // }
}
