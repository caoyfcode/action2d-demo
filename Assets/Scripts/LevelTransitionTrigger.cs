using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO 改名为 LevelSwitchTrigger
public class LevelTransitionTrigger : MonoBehaviour
{
    public Vector2Int targetLevelIndex;
    bool loading = false;
    public TeleportType teleportType;
    public Vector2[] teleportPositions; // 长度为 0, 1, 2

    public enum TeleportType
    {
        None,
        RelativePosition,
        AbsolutePosition,
        RelativePositionByFacing,
        AbsolutePositionByFacing,
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!loading)
        {
            loading = true;

        }
        switch (teleportType)
        {
            case TeleportType.None:
                break;
            case TeleportType.RelativePosition:
                Game.Player.transform.position = Game.Player.transform.position + (Vector3)teleportPositions[0];
                break;
            case TeleportType.AbsolutePosition:
                Game.Player.transform.position = teleportPositions[0];
                break;
            case TeleportType.RelativePositionByFacing:
                if (Game.Player.FacingDirection > 0)
                {
                    Game.Player.transform.position = Game.Player.transform.position + (Vector3)teleportPositions[0];
                }
                else
                {
                    Game.Player.transform.position = Game.Player.transform.position + (Vector3)teleportPositions[1];
                }
                break;
            case TeleportType.AbsolutePositionByFacing:
                if (Game.Player.FacingDirection > 0)
                {
                    Game.Player.transform.position = teleportPositions[0];
                }
                else
                {
                    Game.Player.transform.position = teleportPositions[1];
                }
                break;
        }
        Game.StartChangeLevel(targetLevelIndex);
        Debug.Log("trigger enter");
    }

}
