using UnityEngine;

public class PlanningState : IWaveState
{
    private readonly WaveManager context;

    private float remainingTime;

    public float RemainingTime => remainingTime;


    public PlanningState(WaveManager context)
    {
        this.context = context;
    }


    public void Enter()
    {
        remainingTime = context.ReadyTime;

        context.PrepareWave();

        context.NotifyWaveReady();
    }


    public void Update()
    {
        remainingTime -= Time.deltaTime;

        if (remainingTime <= 0f)
        {
            remainingTime = 0f;

            context.ChangeState(
                new PlayingState(context)
            );
        }
    }


    public void Exit()
    {
    }
}