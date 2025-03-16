using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueWitchController : EntityController
{
    private BlueWitch entity;
    private bool shouldSkip = false; // 当菜单开启时以及刚关闭时，这帧不应该接受输入(skip)

    public override void Init()
    {
        entity = GetComponent<BlueWitch>();
    }

    public override void UpdateCommand()
    {
        if (entity.isDead || UIManager.MenuShown || GameManager.Pause)
        {
            shouldSkip = true;
            return;
        }
        if (!shouldSkip)
        {
            entity.CurrentCommand.speedX = GameInputManager.Horizontal * entity.info.horizontalMoveSpeed;
            entity.CurrentCommand.startJump = GameInputManager.Jump;
            entity.CurrentCommand.holdJump = GameInputManager.JumpHold;
            entity.CurrentCommand.dash = GameInputManager.Dash;
            entity.CurrentCommand.attackHeroKnight = GameInputManager.Attack1;
            entity.CurrentCommand.defendFireKnight = GameInputManager.Attack2;
            entity.CurrentCommand.attackArcaneArcher = GameInputManager.Attack3;
        }
        shouldSkip = false;
    }
}
