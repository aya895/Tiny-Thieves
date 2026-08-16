
public class GameCompleteState : IWaveState
{
    private readonly WaveManager waveManager;

    public GameCompleteState(WaveManager waveManager)
    {
        this.waveManager = waveManager;
    }

    public void Enter()
    {
        waveManager.HandleGameComplete();
    }

    public void Update()
    {
        // Terminal state - nothing left to run.
    }

    public void Exit()
    {
    }
}
