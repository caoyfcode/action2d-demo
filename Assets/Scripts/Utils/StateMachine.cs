public class StateMachine<T> where T: IStateMachineState
{
    public T CurrentState {get; private set;}

    public virtual void Init(T initState)
    {
        CurrentState = initState;
        CurrentState.Enter();
    }

    public void ChangeState(T anotherState)
    {
        CurrentState.Exit();
        CurrentState = anotherState;
        CurrentState.Enter();
    }

    public void Update()
    {
        while (CurrentState.Transition());
        CurrentState.Update();
    }

}

/// <summary>
/// enter -> loop { transition -> update } -> end
/// </summary>
public interface IStateMachineState
{
    void Enter();
    bool Transition(); // read envrionment and do transition
    void Update(); // update action
    void Exit();
}
