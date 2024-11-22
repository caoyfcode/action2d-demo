using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueWitchController : EntityController
{
    private BlueWitch entity;

    public override void Init()
    {
        entity = GetComponent<BlueWitch>();
    }

    public override void UpdateCommand()
    {
        if (entity.isDead || entity.stateMachine.CurrentState == entity.menuState || Game.Pause)
        {
            return;
        }
        bool left = Input.GetKey(KeyCode.A);
        bool right = Input.GetKey(KeyCode.D);
        float moveX = (left, right) switch
        {
            (true, false) => -1,
            (false, true) => 1,
            _ => 0,
        };

        entity.CurrentCommand.speedX = moveX * entity.data.horizontalMoveSpeed;
        entity.CurrentCommand.startJump = Input.GetButtonDown("Jump");
        entity.CurrentCommand.holdJump = Input.GetButton("Jump");
        entity.CurrentCommand.attackHeroKnight = Input.GetButtonDown("Attack");
        entity.CurrentCommand.dash = Input.GetButtonDown("Dash");
        entity.CurrentCommand.attackArcaneArcher = Input.GetKeyDown(KeyCode.U);
        entity.CurrentCommand.defendFireKnight = Input.GetKeyDown(KeyCode.H);
    }
}
