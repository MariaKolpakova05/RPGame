using System;

public interface IState
{
    void Enter();
    void Exit();
    void LogicUpdate();
    void PhysicsUpdate();
}

public class StateMachine
{
    public IState CurrentState { get; private set; }
    public IState PreviousState { get; private set; }

    public event Action<IState> OnStateChanged;

    public void Initialize(IState startState)
    {
        CurrentState = startState;
        CurrentState.Enter();
        OnStateChanged?.Invoke(CurrentState);
    }

    public void ChangeState(IState newState)
    {
        if (newState == null || newState == CurrentState) return;
        PreviousState = CurrentState;
        CurrentState.Exit();
        CurrentState = newState;
        CurrentState.Enter();
        OnStateChanged?.Invoke(CurrentState);
    }

    public void Update() => CurrentState?.LogicUpdate();
    public void FixedUpdate() => CurrentState?.PhysicsUpdate();
}