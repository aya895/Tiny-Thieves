public class PlayingState : IWaveState
{
    private readonly WaveManager waveManager;

    public PlayingState(WaveManager waveManager)
    {
        this.waveManager = waveManager;
    }

    public void Enter()
    {
        waveManager.StartPlayingPhase();
    }

    public void Update()
    {
        waveManager.UpdatePlayingTimer();
    }

    public void Exit()
    {
    }
}