
// Task 用于表示某种行为逻辑, 目前想到的是用来表示实体的原子行为，也可以是 AbilitySystem 内部的行为逻辑，可以作为状态机与行为树的最小结点
// 并且希望可以有各种 Task 子类, 可以是原子行为, 或者是内部包含原子行为的抽象 Task，或者是可以多级嵌套的 Task

using UnityEngine;

public enum GameTaskStatus
{
    UnStarted = 0,
    Running = 1,
    Complete = 2,
    Success = 2,
    Break = 3,
    Failure = 3,
}

[System.Serializable]
public abstract class GameTask
{
    public string name;
    public GameTaskStatus status;

    #region Override Function
    // 作最初的初始化工作
    public virtual void Init()
    {
        name = GetType().Name;
        status = GameTaskStatus.UnStarted;
    }
    public virtual void DeInit() {}
    
    // 每次进入的初始化工作, 与事件注册
    public virtual void OnEnter() {}
    public abstract void OnTick();
    public virtual void OnExit() {}
    
    public virtual void OnBreak() {}
    public virtual void OnComplete() {}
    #endregion

    #region Interface Function
    public void Enter()
    {
        if (status == GameTaskStatus.UnStarted)
        {
            Init();
        }
        status = GameTaskStatus.Running;
        OnEnter();
    }

    public void Tick()
    {
        OnTick();
    }
    
    public void Exit(bool isBreak=false)
    {
        if (isBreak)
        {
            status = GameTaskStatus.Break;
            OnBreak();
        }
        else
        {
            status = GameTaskStatus.Complete;
            OnComplete();
        }
        OnExit();
    }
    
    public void Complete()
    {
        Exit(false);
    }
    
    public void Break()
    {
        Exit(true);
    }
    #endregion
}

public abstract class GameAction : GameTask
{
    public abstract void Call();

    public sealed override void OnTick()
    {
        Call();
        Complete();
    }
}

public abstract class GameCondition : GameTask
{
    public abstract bool Check();
    public sealed override void OnTick()
    {
        var result = Check();
        Exit(result);
    }
}

