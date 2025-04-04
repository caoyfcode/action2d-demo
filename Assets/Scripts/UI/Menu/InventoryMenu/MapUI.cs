using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MapUI : ShownUI
{
    public Map map;

    void Update()
    {
        map.Move(GameInputManager.Move);

        /// TODO 调整 scale 改为单独的按键
        // bool scaleBig = GameInputManager.Attack3;
        // bool scaleSmall = GameInputManager.Jump;
        // float scaleAdd = (scaleBig, scaleSmall) switch
        // {
        //     (true, false) => 1f,
        //     (false, true) => -1f,
        //     _ => 0
        // };
        // map.ChangeScale(scaleAdd * 2);

        if (UIManager.instance.inventoryMenu.Shown && GameInputManager.UiCancel)
        {
            UIManager.instance.inventoryMenu.Shown = false;
            SoundManager.instance.PlayUIConfirm();
        }
    }
}
