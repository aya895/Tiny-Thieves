using System;

public class WaveStateMachine
{
    public IWaveState CurrentState { get; private set; }

    public event Action<IWaveState> OnStateChanged;

    public void ChangeState(IWaveState newState)
    {
        if (newState == null)
            throw new ArgumentNullException(nameof(newState));

        if (ReferenceEquals(CurrentState, newState))
            return;

        CurrentState?.Exit();

        CurrentState = newState;

        CurrentState.Enter();

        OnStateChanged?.Invoke(CurrentState);
    }

    public void Update()
    {
        CurrentState?.Update();
    }

    public bool IsInState<T>() where T : IWaveState
    {
        return CurrentState is T;
    }
}