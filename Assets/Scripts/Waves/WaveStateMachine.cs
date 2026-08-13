public class WaveStateMachine
{
    public IWaveState CurrentState { get; private set; }

    public void ChangeState(IWaveState newState)
    {
        CurrentState?.Exit();

        CurrentState = newState;

        CurrentState?.Enter();
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