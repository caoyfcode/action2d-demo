using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSwitchTrigger : MonoBehaviour
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
        Vector3 targetPosition = Game.Player.transform.position;
        switch (teleportType)
        {
            case TeleportType.None:
                break;
            case TeleportType.RelativePosition:
                targetPosition = Game.Player.transform.position + (Vector3)teleportPositions[0];
                break;
            case TeleportType.AbsolutePosition:
                targetPosition = teleportPositions[0];
                break;
            case TeleportType.RelativePositionByFacing:
                if (Game.Player.FacingDirection > 0)
                {
                    targetPosition = Game.Player.transform.position + (Vector3)teleportPositions[0];
                }
                else
                {
                    targetPosition = Game.Player.transform.position + (Vector3)teleportPositions[1];
                }
                break;
            case TeleportType.AbsolutePositionByFacing:
                if (Game.Player.FacingDirection > 0)
                {
                    targetPosition = teleportPositions[0];
                }
                else
                {
                    targetPosition = teleportPositions[1];
                }
                break;
        }
        Game.Teleport(targetLevelIndex, targetPosition);
    }

}
