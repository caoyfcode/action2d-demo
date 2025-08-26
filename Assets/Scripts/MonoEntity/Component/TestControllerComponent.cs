using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(DetectSurroundComponent), typeof(MoveComponent))]
public class TestControllerComponent : EntityComponent
{
    public float moveSpeed;
    public float jumpSpeed;
    DetectSurroundComponent detectComp;
    MoveComponent moveComp;

    public override void OnEntityAwake()
    {
        detectComp = entity.GetEntityComponent<DetectSurroundComponent>();
        moveComp = entity.GetEntityComponent<MoveComponent>();
    }

    public override void OnEntityUpdate()
    {
        base.OnEntityUpdate();
        var keyboard = Keyboard.current;
        if (keyboard == null)
            return;
        bool left = keyboard.aKey.IsPressed();
        bool right = keyboard.dKey.IsPressed();
        bool jump = keyboard.kKey.wasPressedThisFrame;

        if (left && right)
        {
            left = false;
            right = false;
        }
        float moveX = left ? -1 : (right ? 1 : 0);

        var moveType = new MoveType();
        moveType.directionType = DirectionType.SameAsVelocity;
        if (moveX != 0f)
        {
            if (detectComp.IsOnDirectionWall(moveX))
            {
                moveType.horizontalMoveType = HorizontalMoveType.None;
            }
            else
            {
                moveType.horizontalMoveType = HorizontalMoveType.Speed;
                moveComp.setVelocity.x = moveX * moveSpeed;
            }
        }

        if (detectComp.IsOnGround)
        {
            if (jump)
            {
                moveType.verticalMoveType = VerticalMoveType.Speed;
                moveComp.setVelocity.y = jumpSpeed;
            }
            else
            {
                if (moveComp.FinalVelocity.y > 0)
                {
                    moveType.verticalMoveType = VerticalMoveType.Gravity;
                }
                else
                {
                    moveType.verticalMoveType = VerticalMoveType.None;
                }
            }
        }
        else
        {
            moveType.verticalMoveType = VerticalMoveType.Gravity;
        }
        moveComp.baseMoveType = moveType;
    }
}