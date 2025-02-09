using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class SelectGroup
{
    [System.Serializable]
    public class ItemActionPair
    {
        public SelectItem item;
        public UnityEvent action;
    }

    public List<ItemActionPair> itemActions;
    public int selected = -1; // 初始时什么也不选中

    public void UpdateSelection(bool inc, bool dec)
    {
        int prevSelected = selected;
        if (inc && !dec)
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
        else if (!inc && dec)
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
        // else 即使 selected 无意义，也不改变
        if (prevSelected != selected)
        {
            if (prevSelected >= 0 && prevSelected < itemActions.Count)
            {
                itemActions[prevSelected].item.selected = false;
            }
            if (selected >= 0 && selected < itemActions.Count)
            {
                itemActions[selected].item.selected = true;
                SoundManager.instance.PlayUISelection();
            }
        }
    }

    public void InvokeAction()
    {
        if (selected >= 0 && selected < itemActions.Count)
        {
            itemActions[selected].action?.Invoke();
            SoundManager.instance.PlayUIConfirm();
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

    // public UnityEvent close;

    // Update is called once per frame
    // void Update()
    // {
    //     var items = GetComponentsInChildren<SelectItem>();

    //     if (Input.GetKeyDown(KeyCode.W))
    //     {
    //         selected--;
    //         if (selected < 0)
    //         {
    //             selected = items.Length - 1;
    //         }
    //     }
    //     else if (Input.GetKeyDown(KeyCode.S))
    //     {
    //         selected++;
    //         if (selected > items.Length - 1)
    //         {
    //             selected = 0;
    //         }
    //     }

    //     for (int i = 0; i < items.Length; i++)
    //     {
    //         items[i].selected = selected == i;
    //     }
        
    //     // 取消
    //     if (Input.GetKeyDown(KeyCode.J))
    //     {
    //         Close();
    //     }
    // }

    // public void Close()
    // {
    //     selected = -1; // 重置
    //     var items = GetComponentsInChildren<SelectItem>();
    //     foreach (var item in items)
    //     {
    //         item.ImmediateSelcted(false);
    //     }
    //     close?.Invoke();
    // }
}
