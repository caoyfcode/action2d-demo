using System;
using System.Collections;
using System.Collections.Generic;
using Com.LuisPedroFonseca.ProCamera2D;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Vector2 offset = new(2, 1.6f);
    public enum OffetType
    {
        None,
        Forward,
        Upward,
        ForwardUpward,
    }
    public OffetType offsetType = OffetType.ForwardUpward;

    public ProCamera2D proCamera2D;
    private ProCamera2DShake shakeExt;
    private ProCamera2DNumericBoundaries numericBoundaries;
    // 相机 size 4.876439

    private void Awake()
    {
        shakeExt = proCamera2D.GetComponent<ProCamera2DShake>();
        numericBoundaries = proCamera2D.GetComponent<ProCamera2DNumericBoundaries>();
    }

    private void LateUpdate()
    {
        var target = proCamera2D.GetCameraTarget(Game.Player.transform);
        switch (offsetType)
        {
            case OffetType.None:
                target.TargetOffset = Vector2.zero;
                break;
            case OffetType.Forward:
                target.TargetOffset.x = offset.x * Game.Player.FacingDirection;
                target.TargetOffset.y = 0;
                break;
            case OffetType.Upward:
                target.TargetOffset.x = 0;
                target.TargetOffset.y = offset.y;
                break;
            case OffetType.ForwardUpward:
                target.TargetOffset.x = offset.x * Game.Player.FacingDirection;
                target.TargetOffset.y = offset.y;
                break;
        }
        // target.TargetOffset.x = offset.x * Game.Player.FacingDirection;
        // target.TargetOffset.y = offset.y;
    }

    public void StartShake(Vector2 force, float duration = 0.25f, int vibration = 10)
    {
        shakeExt.Shake(duration, force * 5f, vibration);
    }

    public void SetBoundariesIntantly()
    {
        ProCamera2DRooms rooms = FindAnyObjectByType<ProCamera2DRooms>();
        if (rooms == null)
        {
            Debug.LogError("No ProCamera2DRooms component found in the scene.");
            return;
        }
        var index = rooms.ComputeCurrentRoom(Game.Player.transform.position);
        if (index < 0 || index >= rooms.Rooms.Count)
        {
            Debug.LogError("Invalid room index.");
            return;
        }
        var room = rooms.Rooms[index];
        var numericBoundariesSettings = new NumericBoundariesSettings()
        {
            UseNumericBoundaries = true,
            UseTopBoundary = true,
            TopBoundary = room.Dimensions.y + room.Dimensions.height / 2,
            UseBottomBoundary = true,
            BottomBoundary = room.Dimensions.y  - room.Dimensions.height / 2,
            UseLeftBoundary = true,
            LeftBoundary = room.Dimensions.x - room.Dimensions.width / 2,
            UseRightBoundary = true,
            RightBoundary = room.Dimensions.x + room.Dimensions.width / 2
        };
        numericBoundaries.Settings = numericBoundariesSettings;
    }
}
