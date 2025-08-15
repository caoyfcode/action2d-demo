using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class SelectGroup
{
    // TODO 支持 MxN 的 Grid, UpdateSelection 就改为 UpdateSelection(int horizontal, int vertical=0)， 并支持不同的 WrapMode

    [System.Serializable]
    public class ItemActionPair
    {
        public SelectItem item;
        public UnityEvent action;

        public ItemActionPair(SelectItem item, UnityEvent action)
        {
            this.item = item;
            this.action = action;
        }
    }

    public List<ItemActionPair> itemActions;
    public bool horizontalWrap = false;
    public bool verticalWrap = true;
    public int selected = -1; // 初始时什么也不选中
    public int gridWidth = 1;
    public int GridX => selected % gridWidth;
    public int GridY => selected / gridWidth;
    public int GridXMax => gridWidth - 1;
    public int GridYMax => (itemActions.Count - 1) / gridWidth;

    public bool IsLeftMost => GridX == 0;
    public bool IsRightMost => GridX == GridXMax || selected == itemActions.Count - 1;
    public bool IsTopMost => GridY == 0;
    public bool IsBottomMost => GridY == GridYMax;

    public void Unselect()
    {
        int prevSelected = selected;
        selected = -1;
        if (prevSelected >= 0 && prevSelected < itemActions.Count)
        {
            itemActions[prevSelected].item.selected = false;
        }
    }

    public int IndexOf(int x, int y)
    {
        return y * gridWidth + x;
    }

    public void Select(int x, int y)
    {
        Select(y * gridWidth + x);
    }

    public void Select(int index)
    {
        int prevSelected = selected;
        selected = index;
        if (prevSelected >= 0 && prevSelected < itemActions.Count)
        {
            itemActions[prevSelected].item.selected = false;
        }
        if (selected >= 0 && selected < itemActions.Count)
        {
            itemActions[selected].item.selected = true;
            SoundManager.Ins.PlayUISelection();
        }
    }

    public bool SelectionValid => selected >= 0 && selected < itemActions.Count;

    public void UpdateSelection()
    {
        bool left = GameInputManager.MoveTrigger.x < 0;
        bool right = GameInputManager.MoveTrigger.x > 0;
        bool up = GameInputManager.MoveTrigger.y > 0;
        bool down = GameInputManager.MoveTrigger.y < 0;
        UpdateSelectionWith(left, right, up, down);
    }

    public void UpdateSelectionWith(bool left, bool right, bool up, bool down)
    {
        int prevSelected = selected;
        // 在这些变量中考虑了wrap与否，在之后的实现就无脑wrap
        left = left && (!IsLeftMost || horizontalWrap);
        right = right && (!IsRightMost || horizontalWrap);
        up = up && (!IsTopMost || verticalWrap);
        down = down && (!IsBottomMost || verticalWrap);

        if (left)
        {
            if (selected > 0)
            {
                selected--;
            }
            else
            {
                selected = itemActions.Count - 1;
            }
        }
        else if (right)
        {
            if (selected < itemActions.Count - 1)
            {
                selected++;
            }
            else
            {
                selected = 0;
            }
        }
        else if (up)
        {
            if (selected >= gridWidth)
            {
                selected -= gridWidth;
            }
            else
            {
                // 最顶上再向上，则去往对应水平位置最下方
                int mod = GridX;
                selected = GridYMax * gridWidth + mod;
                // 最后一行可能不满
                if (selected >= itemActions.Count)
                {
                    selected -= gridWidth;
                }
            }
        }
        else if (down)
        {
            if (selected + gridWidth < itemActions.Count)
            {
                selected += gridWidth;
            }
            else
            {
                if (GridY < GridYMax) // 倒数第二行再向下还不到循环，直接到末尾
                {
                    selected = itemActions.Count - 1;
                }
                else // 最后一行再向下
                {
                    selected = GridX;
                }
            }
        }
        // else 即使 selected 无意义，也不改变
        // 计算是否真的改变了并应用动画
        if (prevSelected != selected)
        {
            if (prevSelected >= 0 && prevSelected < itemActions.Count)
            {
                itemActions[prevSelected].item.selected = false;
            }
            if (selected >= 0 && selected < itemActions.Count)
            {
                itemActions[selected].item.selected = true;
                SoundManager.Ins.PlayUISelection();
            }
        }
    }

    // public void UpdateSelection(bool inc, bool dec)
    // {
    //     int prevSelected = selected;
    //     if (inc && !dec)
    //     {
    //         if (selected < itemActions.Count - 1)
    //         {
    //             selected++;
    //         }
    //         else
    //         {
    //             selected = 0;
    //         }
    //     }
    //     else if (!inc && dec)
    //     {
    //         if (selected > 0)
    //         {
    //             selected--;
    //         }
    //         else
    //         {
    //             selected = itemActions.Count - 1;
    //         }
    //     }
    //     // else 即使 selected 无意义，也不改变
    //     if (prevSelected != selected)
    //     {
    //         if (prevSelected >= 0 && prevSelected < itemActions.Count)
    //         {
    //             itemActions[prevSelected].item.selected = false;
    //         }
    //         if (selected >= 0 && selected < itemActions.Count)
    //         {
    //             itemActions[selected].item.selected = true;
    //             SoundManager.instance.PlayUISelection();
    //         }
    //     }
    // }

    public void InvokeAction()
    {
        if (selected >= 0 && selected < itemActions.Count)
        {
            if (itemActions[selected].action != null)
            {
                itemActions[selected].action.Invoke();
                SoundManager.Ins.PlayUIConfirm();
            }
        }
    }

    public void SetSelectedInstantly(int selected)
    {
        this.selected = selected;
        for (int i = 0; i < itemActions.Count; i++)
        {
            itemActions[i].item.SetSelectedInstantly(i == selected);
        }
    }

    public void RefreshItemsFrom(Transform parent)
    {
        itemActions.Clear();
        foreach (Transform child in parent)
        {
            if (child.TryGetComponent<SelectItem>(out var item))
            {
                itemActions.Add(new ItemActionPair(item, null));
            }
        }
        if (selected >= itemActions.Count)
        {
            selected = itemActions.Count - 1;
        }
    }
}
