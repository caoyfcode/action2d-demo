using System;

public class GameTaskComponent : EntityComponent
{
    public GameTask task;
    public ConfigGameTask config;

    public override void OnEntityAwake()
    {
        base.OnEntityAwake();
        task = new GroundMoveTask();
        config = null;
    }

    public override void OnEntityStart()
    {
        base.OnEntityStart();
        
        task.Init(entity, config);
    }

    public override void OnEntityUpdate()
    {
        task.Tick();
    }
}