using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
            entity.CurrentCommand.dash = DataManager.instance.PlayerData.ability.dash && GameInputManager.Dash; // 前一个判断不是很必要，因为 entity 内有判断
            entity.CurrentCommand.attackHeroKnight = false;
            entity.CurrentCommand.defendFireKnight = false;
            entity.CurrentCommand.attackArcaneArcher = false;
            UpdateAttack(DataManager.instance.PlayerData.controlConfig.attack1, GameInputManager.Attack1);
            UpdateAttack(DataManager.instance.PlayerData.controlConfig.attack2, GameInputManager.Attack2);
            UpdateAttack(DataManager.instance.PlayerData.controlConfig.attack3, GameInputManager.Attack3);
            // entity.CurrentCommand.attackHeroKnight = GameInputManager.Attack1;
            // entity.CurrentCommand.defendFireKnight = GameInputManager.Attack2;
            // entity.CurrentCommand.attackArcaneArcher = GameInputManager.Attack3;
        }
        shouldSkip = false;
    }

    void UpdateAttack(string id, bool value)
    {
        switch (id)
        {
            case ItemData.HeroKnightItemId:
                entity.CurrentCommand.attackHeroKnight |= value;
                break;
            case ItemData.FireKnightItemId:
                entity.CurrentCommand.defendFireKnight |= value;
                break;
            case ItemData.ArcaneArcherItemId:
                entity.CurrentCommand.attackArcaneArcher |= value;
                break;
            default:
                // do nothing
                break;
        }
    }
}
