public class UpgradeState : IWaveState
{
    private readonly WaveManager waveManager;

    public UpgradeState(WaveManager waveManager)
    {
        this.waveManager = waveManager;
    }

    public void Enter()
    {
        waveManager.StartUpgradePhase();
    }

    public void Update()
    {
    }

    public void Exit()
    {
    }
}