using System;
using System.Collections;
using System.Collections.Generic;
using Com.LuisPedroFonseca.ProCamera2D;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float forwardOffset = 2;
    public float upwardOffset = 1.6f;
    public float downwardOffset = 0.5f;
    public enum OffetType
    {
        None,
        Forward,
        Upward,
        Downward,
        ForwardUpward,
        ForwardDownward,
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
        offsetType = OffetType.ForwardUpward;
    }

    void Update()
    {
        // 到达边缘时降低镜头，直到不在边缘且在地面
        if (WorldManager.Player.IsFacingCliff)
        {
            offsetType = OffetType.ForwardDownward;
        }
        else if (WorldManager.Player.isOnGround)
        {
            offsetType = OffetType.ForwardUpward;
        }
    }

    private void LateUpdate()
    {
        var target = proCamera2D.GetCameraTarget(WorldManager.Player.transform);
        switch (offsetType)
        {
            case OffetType.None:
                target.TargetOffset = Vector2.zero;
                break;
            case OffetType.Forward:
                target.TargetOffset.x = forwardOffset * WorldManager.Player.FacingDirection;
                target.TargetOffset.y = 0;
                break;
            case OffetType.Upward:
                target.TargetOffset.x = 0;
                target.TargetOffset.y = upwardOffset;
                break;
            case OffetType.Downward:
                target.TargetOffset.x = 0;
                target.TargetOffset.y = -downwardOffset;
                break;
            case OffetType.ForwardUpward:
                target.TargetOffset.x = forwardOffset * WorldManager.Player.FacingDirection;
                target.TargetOffset.y = upwardOffset;
                break;
            case OffetType.ForwardDownward:
                target.TargetOffset.x = forwardOffset * WorldManager.Player.FacingDirection;
                target.TargetOffset.y = -downwardOffset;
                break;
        }
    }

    public void StartShake(Vector2 force, float duration = 0.25f, int vibration = 10)
    {
        shakeExt.Shake(duration, force * 5f, vibration);
    }

    public void MoveInstantlyAndSetBoundaries()
    {
        proCamera2D.CenterOnTargets();
        ProCamera2DRooms rooms = FindAnyObjectByType<ProCamera2DRooms>();
        if (rooms == null)
        {
            Debug.LogError("No ProCamera2DRooms component found in the scene.");
            return;
        }
        var index = rooms.ComputeCurrentRoom(WorldManager.Player.transform.position);
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
            RightBoundary = room.Dimensions.x + room.Dimensions.width / 2,
        };
        numericBoundaries.Settings = numericBoundariesSettings;
    }

    // 用来当某些房间过大时设置边界，在加载新 Level 后调用
    // 不论 Room 是否设置了 ScaleCameraToFit, ProCamera2D 都会在房间过小时将相机缩小
    // 设置了 ScaleCameraToFit, ProCamera2D 会在房间过大时将相机扩大
    // 因而此函数相当于在 ScaleCameraToFit 为 false 时, 在房间过小时扩大房间，从而无需改变相机大小
    // 其实是令 ScaleCameraToFit=false 在在房间过小时仍然起作用
    public void FitRoomSize()
    {
        ProCamera2DRooms rooms = FindAnyObjectByType<ProCamera2DRooms>();
        if (rooms == null)
        {
            Debug.LogError("No ProCamera2DRooms component found in the scene.");
            return;
        }
        foreach (var room in rooms.Rooms)
        {
            if (room.ScaleCameraToFit)
            {
                break;
            }
            if (room.Dimensions.width < proCamera2D.ScreenSizeInWorldCoordinates.x)
            {
                room.Dimensions.width = proCamera2D.ScreenSizeInWorldCoordinates.x + 0.1f; // 加0.1是为了防止浮点误差使得将摄像机稍微缩小
            }
            if (room.Dimensions.height < proCamera2D.ScreenSizeInWorldCoordinates.y)
            {
                room.Dimensions.height = proCamera2D.ScreenSizeInWorldCoordinates.y + 0.1f;
            }
        }
    }

    // 用于设置 Room 扩展切换 Room 依据的 transform
    // 默认的是带有 offset 的, 而此游戏不希望有 offset
    public void SetRoomTriggerTarget()
    {
        ProCamera2DRooms rooms = FindAnyObjectByType<ProCamera2DRooms>();
        if (rooms == null)
        {
            Debug.LogError("No ProCamera2DRooms component found in the scene.");
            return;
        }
        rooms.TriggerTarget = WorldManager.Player.transform;
    }
}
