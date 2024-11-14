using System;
using System.Collections;
using System.Collections.Generic;
using Com.LuisPedroFonseca.ProCamera2D;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Vector2 offset = new(2, 1.6f);

    public ProCamera2D proCamera2D;
    private ProCamera2DShake shakeExt;
    // 相机 size 4.876439

    private void Awake()
    {
        shakeExt = proCamera2D.GetComponent<ProCamera2DShake>();
    }

    private void LateUpdate()
    {
        var target = proCamera2D.GetCameraTarget(Game.Player.transform);
        target.TargetOffset.x = offset.x * Game.Player.FacingDirection;
        target.TargetOffset.y = offset.y;
    }

    public void StartShake(Vector2 force, float duration = 0.25f, int vibration = 10)
    {
        shakeExt.Shake(duration, force * 5f, vibration);
    }
}
