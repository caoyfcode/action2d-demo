using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SelectGroup : MonoBehaviour
{
    public int selected = -1; // 初始时什么也不显示
    public UnityEvent close;

    // Update is called once per frame
    void Update()
    {
        var items = GetComponentsInChildren<SelectItem>();

        if (Input.GetKeyDown(KeyCode.W))
        {
            selected--;
            if (selected < 0)
            {
                selected = items.Length - 1;
            }
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            selected++;
            if (selected > items.Length - 1)
            {
                selected = 0;
            }
        }

        for (int i = 0; i < items.Length; i++)
        {
            items[i].selected = selected == i;
        }
        
        // 取消
        if (Input.GetKeyDown(KeyCode.J))
        {
            Close();
        }
    }

    public void Close()
    {
        selected = -1; // 重置
        var items = GetComponentsInChildren<SelectItem>();
        foreach (var item in items)
        {
            item.ImmediateSelcted(false);
        }
        close?.Invoke();
    }
}
