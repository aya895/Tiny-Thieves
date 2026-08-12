using System.Collections;
using TMPro;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [Header("Time Settings")]
    [SerializeField] private float readyTime = 10f;
    [SerializeField] private float waveDuration = 60f;

    //--------------------------------------- (new) count down before each wave start (also handles the first tnt placement when clicking any button)
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private int countdownSeconds = 3;
    //----------------------------------------

    [Header("References")]
    [SerializeField] private SpawnManager spawnManager;

    public float ReadyTime => readyTime;
    public float WaveDuration => waveDuration;
    public float RemainingTime => timer;
    public int CurrentWave { get; private set; }
    public WaveState CurrentState { get; private set; }
    private float timer;

    
    private void OnEnable()
    {
        // The only thing WaveManager knows about the upgrade flow: "someone
        // will tell me when it's resolved." It never references
        // ExperienceManager or UpgradeSelectionUI directly.
        UpgradeFlowSignal.OnResolved += HandleUpgradesResolved;
    }
    private void OnDisable()
    {
        UpgradeFlowSignal.OnResolved -= HandleUpgradesResolved;
    }

    private void Start()
    {
        CurrentWave = 0;
        //CurrentState = WaveState.WaitingToStart;

        StartCoroutine(StartCountdownSequence(countdownSeconds)); // just start without any buttons to be pressed 
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
    // used in play button 
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
        spawnManager.ClearPreviousWave();
        if (CurrentState != WaveState.WaitingToStart)
            return;

        //StartReadyPhase();
        StartCoroutine(StartCountdownSequence(countdownSeconds));
    }


    private void StartReadyPhase()
    {
        CurrentState = WaveState.Ready;
        timer = readyTime;

        Debug.Log("Planning Phase Started");

        WaveReadySignal.Raise();
    }

    private void StartWave()
    {
        CurrentWave++;

        CurrentState = WaveState.Playing;
        timer = waveDuration;

        Debug.Log($"Wave {CurrentWave} Started");

        if(spawnManager != null)
        {
            spawnManager.StartWave();
        }
    }

    private void EndWave()
    {
        spawnManager.ClearPreviousWave();
        CurrentState = WaveState.Upgrade;

        Debug.Log($"Wave {CurrentWave} Completed");

        // Whether the player leveled up zero times or five times this wave
        // is none of WaveManager's business - it just announces the wave
        // is over and waits to be told it can continue.
        WaveEndSignal.Raise();
    }

    // Fires when ExperienceManager (nothing pending) or UpgradeSelectionUI
    // (player finished picking) says the post-wave upgrade step is done.
    private void HandleUpgradesResolved()
    {
        if (CurrentState != WaveState.Upgrade)
            return; // ignore stray/duplicate signals outside the Upgrade state

        //StartReadyPhase();
        StartCoroutine(StartCountdownSequence(countdownSeconds));
    }

    private IEnumerator StartCountdownSequence(int num)
    {
        countdownText.gameObject.SetActive(true);
        for (int i = num; i > 0; i--)
        {
            if (countdownText != null)
            {
                countdownText.text = i.ToString();
            }
            yield return new WaitForSeconds(1f);
        }
        countdownText.text = "START!";
        yield return new WaitForSeconds(0.5f);
        countdownText.gameObject.SetActive(false);
        StartReadyPhase();
    }

    public void FinishUpgrade()
    {
        //StartReadyPhase();
        StartCoroutine(StartCountdownSequence(countdownSeconds));
    }

    public void GameOver()
    {
        CurrentState = WaveState.GameOver;

        Debug.Log("GAME OVER");

        // Later: Show Game Over Screen
    }
}