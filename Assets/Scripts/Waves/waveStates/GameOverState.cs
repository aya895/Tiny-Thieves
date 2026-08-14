public class GameOverState : IWaveState
{
    private readonly WaveManager waveManager;

    public GameOverState(WaveManager waveManager)
    {
        this.waveManager = waveManager;
    }

    public void Enter()
    {
        waveManager.HandleGameOver();
    }

    public void Update()
    {
    }

    public void Exit()
    {
    }
}