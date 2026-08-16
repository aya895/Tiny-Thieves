using UnityEngine;

public class PlayingState : IWaveState
{
    private readonly WaveManager context;

    private float remainingTime;

    public float RemainingTime => remainingTime;


    public PlayingState(WaveManager context)
    {
        this.context = context;
    }


    public void Enter()
    {
        remainingTime = context.WaveDuration;

        context.BeginWave();
    }


    public void Update()
    {
        if (context.IsWaveEndLocked)
            return;

        remainingTime -= Time.deltaTime;

        if (remainingTime <= 0f)
        {
            remainingTime = 0f;

            context.CompleteWave();
        }
    }


    public void Exit()
    {
    }
}