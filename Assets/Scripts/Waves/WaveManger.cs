using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [Header("Time Settings")]
    [SerializeField] private float readyTime = 10f;
    [SerializeField] private float waveDuration = 60f;

    [Header("References")]
    [SerializeField] private SpawnManager spawnManager;

    public float ReadyTime => readyTime;
    public float WaveDuration => waveDuration;
    public float RemainingTime => timer;

    public int CurrentWave { get; private set; }

    public WaveState CurrentState { get; private set; }

    private float timer;

    private void Start()
    {
        CurrentWave = 0;
        CurrentState = WaveState.WaitingToStart;
    }

    private void Update()
    {
        if (CurrentState == WaveState.GameOver)
            return;

        if (CurrentState != WaveState.Ready &&
            CurrentState != WaveState.Playing)
            return;

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            HandleTimerFinished();
        }
    }

    private void HandleTimerFinished()
    {
        switch (CurrentState)
        {
            case WaveState.Ready:
                StartWave();
                break;

            case WaveState.Playing:
                EndWave();
                break;
        }
    }

    // Called by Start Button
    public void StartPlanning()
    {
        if (CurrentState != WaveState.WaitingToStart)
            return;

        StartReadyPhase();
    }

    private void StartReadyPhase()
    {
        CurrentState = WaveState.Ready;
        timer = readyTime;

        Debug.Log("Planning Phase Started");
    }

    private void StartWave()
    {
        CurrentWave++;

        CurrentState = WaveState.Playing;
        timer = waveDuration;

        Debug.Log($"Wave {CurrentWave} Started");

        spawnManager.StartWave();
    }

    private void EndWave()
    {
        CurrentState = WaveState.Upgrade;

        Debug.Log($"Wave {CurrentWave} Completed");

        // Later: Show Upgrade Screen
    }

    public void FinishUpgrade()
    {
        StartReadyPhase();
    }

    public void GameOver()
    {
        CurrentState = WaveState.GameOver;

        Debug.Log("GAME OVER");

        // Later: Show Game Over Screen
    }
}