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
        UIManager.instance.inventoryMenu.footHint.SetHints(new(){FootHint.FootHintType.ZoomUp, FootHint.FootHintType.ZoomDown});

        /// TODO 调整 scale 改为单独的按键
        bool zoomUp = GameInputManager.UIMapZoomUp;
        bool zoomDown = GameInputManager.UIMapZoomDown;
        float scaleAdd = (zoomUp, zoomDown) switch
        {
            (true, false) => 1f,
            (false, true) => -1f,
            _ => 0
        };
        map.ChangeScale(scaleAdd * 2);
    }
}
