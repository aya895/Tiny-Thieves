public class UpgradeState : IWaveState
{
    private readonly WaveManager context;


    public UpgradeState(WaveManager context)
    {
        this.context = context;
    }


    public void Enter()
    {
        context.BeginUpgradePhase();
    }


    public void Update()
    {
    }


    public void Exit()
    {
    }
}