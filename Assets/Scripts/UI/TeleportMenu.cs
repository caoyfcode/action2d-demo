using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportMenu : MonoBehaviour
{
    // TODO 将 Shown 的实现放入 UIBase 或 ShownUI 中
    public bool Shown
    {
        get { return gameObject.activeSelf; }
        set
        {
            if (!gameObject.activeSelf && value)
            {
                Game.Player.stateMachine.ChangeState(Game.Player.menuState);
            }
            else if (gameObject.activeSelf && !value)
            {
                Game.Player.stateMachine.ChangeState(Game.Player.groundFreeState);
            }
            gameObject.SetActive(value);
        }
    }

    public void Close()
    {
        Shown = false;
    }
}
