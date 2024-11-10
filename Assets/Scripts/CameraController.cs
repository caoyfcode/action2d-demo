using System;
using System.Collections;
using System.Collections.Generic;
using Com.LuisPedroFonseca.ProCamera2D;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Vector2 offset = new(2, 1.6f);

    public ProCamera2D proCamera2D;
    private ProCamera2DNumericBoundaries numericBoundaries;
    private ProCamera2DShake shake;


    private void Awake()
    {
        numericBoundaries = proCamera2D.GetComponent<ProCamera2DNumericBoundaries>();
        shake = proCamera2D.GetComponent<ProCamera2DShake>();
    }

    private void LateUpdate()
    {
        var target = proCamera2D.GetCameraTarget(Game.Player.transform);
        target.TargetOffset.x = offset.x * Game.Player.FacingDirection;
        target.TargetOffset.y = offset.y;
    }

    public void StartShake(Vector2 force, float duration = 0.25f, int vibration = 10)
    {
        shake.Shake(duration, force * 5f, vibration);
    }

    public void SetBound(Collider2D bound)
    {
        if (bound == null)
        {
            numericBoundaries.UseTopBoundary = false;
            numericBoundaries.UseBottomBoundary = false;
            numericBoundaries.UseLeftBoundary = false;
            numericBoundaries.UseRightBoundary = false;
        }
        else
        {
            numericBoundaries.UseTopBoundary = true;
            numericBoundaries.TopBoundary = bound.bounds.max.y;
            numericBoundaries.UseBottomBoundary = true;
            numericBoundaries.BottomBoundary = bound.bounds.min.y;
            numericBoundaries.UseLeftBoundary = true;
            numericBoundaries.LeftBoundary = bound.bounds.min.x;
            numericBoundaries.UseRightBoundary = true;
            numericBoundaries.RightBoundary = bound.bounds.max.x;
        }
    }
}
