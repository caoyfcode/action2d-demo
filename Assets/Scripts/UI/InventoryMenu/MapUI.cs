using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapUI : ShownUI
{
    public Map map;

    void Update()
    {
        map.Move(GameInputManager.Move);

        bool scaleBig = GameInputManager.Attack3;
        bool scaleSmall = GameInputManager.Jump;
        float scaleAdd = (scaleBig, scaleSmall) switch
        {
            (true, false) => 1f,
            (false, true) => -1f,
            _ => 0
        };
        map.ChangeScale(scaleAdd * 2);
    }
}
