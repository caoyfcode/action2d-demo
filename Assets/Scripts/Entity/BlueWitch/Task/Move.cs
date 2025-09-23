using System;
using UnityEngine;

public class GroundMoveTask : GameTask
{
    public ConfigGroundMoveTask config;
    private MoveComponent moveComp;
    private AnimatorComponent animComp;
    private DetectSurroundComponent detectComp;
    
    public override void Init(MonoEntity entity, ConfigGameTask config)
    {
        base.Init(entity, config);
        this.config = (ConfigGroundMoveTask)config;
        moveComp = entity.GetEntityComponent<MoveComponent>();
        animComp = entity.GetEntityComponent<AnimatorComponent>();
        detectComp = entity.GetEntityComponent<DetectSurroundComponent>();
    }

    public override GameTaskStatus OnTick()
    {
        var input = GameInputManager.Move;
        var moveX = input.x;
        moveComp.setVelocity = Vector2.zero;

        if (moveX == 0)
        {
            animComp.ChangeAnimation("Idle");
            moveComp.baseMoveType = MoveType.None;
        }
        else
        {
            animComp.ChangeAnimation("Run");
            moveComp.baseMoveType = MoveType.Horizontal;
            if (moveX < 0)
            {
                moveComp.setVelocity.x = -5f;
            }
            else
            {
                moveComp.setVelocity.x = 5f;
            }
        }
        
        return GameTaskStatus.Running;
    }
}

[System.Serializable]
public class ConfigGroundMoveTask : ConfigGameTask
{
    
}