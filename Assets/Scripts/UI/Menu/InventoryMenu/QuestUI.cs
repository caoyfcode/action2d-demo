using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestUI : ShownUI
{
    // TODO 实现任务列表以及任务目标位置指示

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnEnable()
    {
        UIManager.Ins.inventoryMenu.footHint.SetHints(new()); // 空
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
