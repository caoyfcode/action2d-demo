using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : ShownUI
{
    public enum SelectedTable
    {
        None,
        AttackConfig,
        Attack,
        Item,
    }

    public GameObject attackConfig;
    public GameObject attackTable;
    public GameObject itemTable;
    public GameObject description;

    public SelectedTable selectedTable = SelectedTable.None;
    public int selectedId = -1;

    void Update()
    {
        UpdateSelection();
        HandleInput();
    }

    void HandleInput()
    {
        if (selectedTable == SelectedTable.Attack)
        {
            if (Input.GetKeyDown(KeyCode.J))
            {
                // TODO 设置 J 为 selectedId 的技能
                SoundManager.instance.PlayUIConfirm();
            }
            if (Input.GetKeyDown(KeyCode.H))
            {
                // TODO 设置 H 为 selectedId 的技能
                SoundManager.instance.PlayUIConfirm();
            }
            if (Input.GetKeyDown(KeyCode.U))
            {
                // TODO 设置 U 为 selectedId 的技能
                SoundManager.instance.PlayUIConfirm();
            }
        }
    }

    void ChangeDescription()
    {
        // TODO 完善此函数
        // string name = "";
        // string description = "";
    }

    void UpdateSelection()
    {
        bool left = Input.GetKeyDown(KeyCode.A);
        bool right = Input.GetKeyDown(KeyCode.D);
        bool up = Input.GetKeyDown(KeyCode.W);
        bool down = Input.GetKeyDown(KeyCode.S);
        
        // 处理前一帧不可用的选中
        if (selectedTable != SelectedTable.None)
        {
            GameObject table = selectedTable switch 
            {
                SelectedTable.Attack => attackTable,
                SelectedTable.Item => itemTable,
                _ => attackConfig,
            };
            if (selectedId < 0 || selectedId >= table.transform.childCount)
            {
                selectedTable = SelectedTable.None;
                selectedId = -1;
            }
        }

        // 计算新的选中
        int prevSelected = selectedId;
        SelectedTable prevSelectedTable = selectedTable;
        if (prevSelectedTable == SelectedTable.None) // 无旧选项
        {
            if (left || right || up || down)
            {
                selectedTable = SelectedTable.AttackConfig;
                selectedId = 0;
            }
        }
        else if ((up && !down) || (down && !up))  // 旧选项存在，且上下运动
        {
            if (prevSelectedTable != SelectedTable.AttackConfig)
            {
                GameObject table = prevSelectedTable == SelectedTable.Attack ? attackTable : itemTable;
                if (up && prevSelected >= 4)
                {
                    selectedId -= 4;
                }
                else if (down)
                {
                    if (prevSelected + 4 < table.transform.childCount)
                        selectedId += 4;
                    else if ((table.transform.childCount - 1) / 4 > prevSelected / 4)
                        selectedId = table.transform.childCount - 1;
                }
            }
        }
        else if ((left && !right) || (right && !left)) // 旧选项存在，且左右运动
        {
            if (prevSelectedTable  == SelectedTable.AttackConfig)
            {
                if (left && prevSelected > 0)
                {
                    selectedId--;
                }
                else if (right)
                {
                    if (prevSelected < attackConfig.transform.childCount - 1)
                    {
                        selectedId++;
                    }
                    else if (attackTable.transform.childCount > 0)
                    {
                        selectedTable = SelectedTable.Attack;
                        selectedId = 0;
                    }
                    else if (itemTable.transform.childCount > 0)
                    {
                        selectedTable = SelectedTable.Item;
                        selectedId = 0;
                    }
                }
            }
            else if (prevSelectedTable  == SelectedTable.Attack)
            {
                int div = prevSelected / 4;
                int mod = prevSelected % 4;
                if (left)
                {
                    if (mod == 0)
                    {
                        selectedTable = SelectedTable.AttackConfig;
                        selectedId = attackConfig.transform.childCount - 1;
                    }
                    else
                    {
                        mod--;
                        selectedId = div * 4 + mod;
                    }
                }
                else // right
                {
                    if (mod == 3 || prevSelected == attackTable.transform.childCount - 1)
                    {
                        if (itemTable.transform.childCount > 0)
                        {
                            selectedTable = SelectedTable.Item;
                            selectedId = Mathf.Min(4 * div, (itemTable.transform.childCount - 1) / 4 * 4);
                        }
                    }
                    else
                    {
                        mod++;
                        selectedId = div * 4 + mod;
                    }
                }
            }
            else // item table
            {
                int div = prevSelected / 4;
                int mod = prevSelected % 4;
                if (right)
                {
                    if (mod == 3 || prevSelected == itemTable.transform.childCount - 1)
                    {
                        // nothing
                    }
                    else
                    {
                        mod++;
                        selectedId = div * 4 + mod;
                    }
                }
                else // left
                {
                    if (mod > 0)
                    {
                        mod--;
                        selectedId = div * 4 + mod;
                    }
                    else if (attackTable.transform.childCount > 0)
                    {
                        selectedTable = SelectedTable.Attack;
                        selectedId = Mathf.Min(4 * div + 3, attackTable.transform.childCount - 1);
                    }
                    else if (attackConfig.transform.childCount > 0)
                    {
                        selectedTable = SelectedTable.AttackConfig;
                        selectedId = attackConfig.transform.childCount - 1;
                    }
                }
            }
        }
        // 旧选项存在且与新选项不同时，取消旧选项
        if (prevSelectedTable != SelectedTable.None &&
            (prevSelectedTable != selectedTable || prevSelected != selectedId))
        {
            GameObject pervTable = prevSelectedTable switch 
            {
                SelectedTable.Attack => attackTable,
                SelectedTable.Item => itemTable,
                _ => attackConfig,
            };
            pervTable.transform.GetChild(prevSelected).GetComponent<SelectItem>().selected = false;
        }
        // 新选项存在且与旧选项不同时，选中新选项
        if (selectedTable != SelectedTable.None &&
            (prevSelectedTable != selectedTable || prevSelected != selectedId))
        {
            GameObject table = selectedTable switch
            {
                SelectedTable.Attack => attackTable,
                SelectedTable.Item => itemTable,
                _ => attackConfig,
            };
            table.transform.GetChild(selectedId).GetComponent<SelectItem>().selected = true;
            ChangeDescription();
            SoundManager.instance.PlayUISelection();
        }
    }
}
