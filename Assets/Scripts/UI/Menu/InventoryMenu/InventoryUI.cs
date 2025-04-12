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
        SelectAttackTableForAttackConfig,
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

    public override bool Shown
    {
        get { return gameObject.activeSelf; }
        set
        {
            bool prev = gameObject.activeSelf;
            gameObject.SetActive(value);
            if (!prev && value)
            {
                state = State.SelectNone;
                RefreshAttackConfig();
                RefreshAttackTable();
                RefreshItemTable();
            }
        }
    }
    void OnEnable()
    {
        state = State.SelectNone;
        RefreshAttackConfig();
        RefreshAttackTable();
        RefreshItemTable();
    }

    void Update()
    {
        UpdateSelection();
        UpdateDescription();
        HandleInput();
        UpdateFootHint(); // TODO 不一定每帧都调用
    }

    void HandleInput()
    {
        if (UIManager.instance.inventoryMenu.Shown)
        {
            if (GameInputManager.UiCancel && state != State.SelectAttackConfigForAttackTable && state != State.SelectAttackTableForAttackConfig)
            {
                UIManager.instance.inventoryMenu.Shown = false;
                SoundManager.instance.PlayUIConfirm();
                return;
            }

            switch (state)
            {
                case State.SelectAttackConfig:
                    if (attackConfig.SelectionValid && GameInputManager.UiConfirm && attackTable.itemActions.Count > 0)
                    {
                        state = State.SelectAttackTableForAttackConfig;
                        string id = attackConfigRoot.transform.GetChild(attackConfig.selected).GetComponent<InventoryItemUI>().item.id;
                        attackTable.Unselect();
                        for (int i = 0; i < attackTableRoot.transform.childCount; i++)
                        {
                            var itemUI = attackTableRoot.transform.GetChild(i).GetComponent<InventoryItemUI>();
                            if (itemUI.item.id == id)
                            {
                                attackTable.Select(i);
                                break;
                            }
                        }
                        if (!attackTable.SelectionValid)
                        {
                            attackTable.Select(0);
                        }
                    }
                    break;
                case State.SelectAttackTable:
                    if (attackTable.SelectionValid && GameInputManager.UiConfirm)
                    {
                        state = State.SelectAttackConfigForAttackTable;
                        attackConfig.Select(0);
                    }
                    break;
                case State.SelectAttackConfigForAttackTable:
                    if (attackConfig.SelectionValid && GameInputManager.UiConfirm)
                    {
                        string id = attackTableRoot.transform.GetChild(attackTable.selected).GetComponent<InventoryItemUI>().item.id;
                        WorldManager.instance.ChangeAttackConfig(attackConfig.selected, id);
                        state = State.SelectAttackTable;
                        attackConfig.Unselect();
                    }
                    else if (GameInputManager.UiCancel)
                    {
                        state = State.SelectAttackTable;
                        attackConfig.Unselect();
                    }
                    break;
                case State.SelectAttackTableForAttackConfig:
                    if (attackTable.SelectionValid && GameInputManager.UiConfirm)
                    {
                        string id = attackTableRoot.transform.GetChild(attackTable.selected).GetComponent<InventoryItemUI>().item.id;
                        WorldManager.instance.ChangeAttackConfig(attackConfig.selected, id);
                        state = State.SelectAttackConfig;
                        attackTable.Unselect();
                    }
                    else if (GameInputManager.UiCancel)
                    {
                        state = State.SelectAttackConfig;
                        attackTable.Unselect();
                    }
                    break;
            }
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
            case State.SelectAttackTableForAttackConfig:
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
        descriptionContent.text = "";
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
                        // TODO 考虑 scroll
                        if (attackTable.IndexOf(attackTable.GridXMax, itemTable.GridY) < attackTable.itemActions.Count)
                            attackTable.Select(attackTable.GridXMax, itemTable.GridY); 
                        else
                            attackTable.Select(attackTable.itemActions.Count - 1);
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
            case State.SelectAttackTableForAttackConfig:
                attackTable.UpdateSelection();
                break;
        }
    }


    public void UpdateFootHint()
    {
        var footHint = UIManager.instance.inventoryMenu.footHint;
        switch (state)
        {
            case State.SelectNone:
            case State.SelectItemTable:
                footHint.SetHints(new());
                break;
            case State.SelectAttackConfig:
                footHint.SetHints(new(){FootHint.FootHintType.EquipSwitch});
                break;
            case State.SelectAttackTable:
                footHint.SetHints(new(){FootHint.FootHintType.Equip});
                break;
            case State.SelectAttackConfigForAttackTable:
                footHint.SetHints(new(){FootHint.FootHintType.EquipSwitch, FootHint.FootHintType.Cancel});
                break;
            
            case State.SelectAttackTableForAttackConfig:
                footHint.SetHints(new(){FootHint.FootHintType.Equip, FootHint.FootHintType.Cancel});
                break;
        }
            
    }

    public void RefreshAttackConfig()
    {
        var attack1 = attackConfigRoot.transform.GetChild(0).GetComponent<InventoryItemUI>();
        var attack2 = attackConfigRoot.transform.GetChild(1).GetComponent<InventoryItemUI>();
        var attack3 = attackConfigRoot.transform.GetChild(2).GetComponent<InventoryItemUI>();
        if (ItemData.AttackItemIds.Contains(DataManager.instance.PlayerData.controlConfig.attack1))
        {
            attack1.SetItem(new InventoryItem { id = DataManager.instance.PlayerData.controlConfig.attack1, quantity = 1});
        }
        else
        {
            attack1.UnsetItem();
        }
        if (ItemData.AttackItemIds.Contains(DataManager.instance.PlayerData.controlConfig.attack2))
        {
            attack2.SetItem(new InventoryItem { id = DataManager.instance.PlayerData.controlConfig.attack2, quantity = 1 });
        }
        else
        {
            attack2.UnsetItem();
        }
        if (ItemData.AttackItemIds.Contains(DataManager.instance.PlayerData.controlConfig.attack3))
        {
            attack3.SetItem(new InventoryItem { id = DataManager.instance.PlayerData.controlConfig.attack3, quantity = 1 });
        }
        else
        {
            attack3.UnsetItem();
        }
        attackConfig.RefreshItemsFrom(attackConfigRoot.transform); // 固定 3 个, 每次修改攻击就刷新
    }

    public void RefreshAttackTable()
    {
        int childCount = attackTableRoot.transform.childCount;
        var attackItems = DataManager.instance.PlayerData.ability.GetAttackItems();
        for (int i = 0; i < attackItems.Count && i < childCount; i++)
        {
            var itemObj = attackTableRoot.transform.GetChild(i).gameObject;
            itemObj.GetComponent<InventoryItemUI>().SetItem(attackItems[i]);
        }
        if (attackItems.Count > childCount)
        {
            for (int i = childCount; i < attackItems.Count; i++)
            {
                var itemObj = Instantiate(itemPrefab, attackTableRoot.transform);
                itemObj.GetComponent<InventoryItemUI>().SetItem(attackItems[i]);
            }
        }
        else if (attackItems.Count < childCount)
        {
            for (int i = childCount - 1; i >= attackItems.Count; i--)
            {
                DestroyImmediate(attackTableRoot.transform.GetChild(i).gameObject);
            }
        }
        attackTable.RefreshItemsFrom(attackTableRoot.transform); // 每次技能更新就刷新
    }

    public void RefreshItemTable()
    {
        int childCount = itemTableRoot.transform.childCount;
        List<InventoryItem> items = new();
        // 包含被动能力以及物品列表
        items.AddRange(DataManager.instance.PlayerData.ability.GetAbilityItems());
        items.AddRange(DataManager.instance.PlayerData.inventory);
        for (int i = 0; i < items.Count && i < childCount; i++)
        {
            var itemObj = itemTableRoot.transform.GetChild(i).gameObject;
            itemObj.GetComponent<InventoryItemUI>().SetItem(items[i]);
        }
        if (items.Count > childCount)
        {
            for (int i = childCount; i < items.Count; i++)
            {
                var itemObj = Instantiate(itemPrefab, itemTableRoot.transform);
                itemObj.GetComponent<InventoryItemUI>().SetItem(items[i]);
            }
        }
        else if (items.Count < childCount)
        {
            for (int i = childCount - 1; i >= items.Count; i++)
            {
                DestroyImmediate(itemTableRoot.transform.GetChild(i).gameObject);
            }
        }
        itemTable.RefreshItemsFrom(itemTableRoot.transform); // 每次拾取物品就刷新
    }
}
