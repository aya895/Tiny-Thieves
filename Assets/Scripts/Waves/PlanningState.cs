public class PlanningState : IWaveState
{
    private readonly WaveManager waveManager;

    public PlanningState(WaveManager waveManager)
    {
        this.waveManager = waveManager;
    }

    public void Enter()
    {
        waveManager.StartPlanningPhase();
    }

    public void Update()
    {
        waveManager.UpdatePlanningTimer();
    }

    public void Exit()
    {
    }
}