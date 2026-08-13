using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
public class WaveManager : MonoBehaviour
{

    public static event Action OnVictory;

    [Header("Wave Victory Check")]
    private int activeAnts = 0;
    private bool isSpawningFinished = false;

    [Header("Time Settings")]
    [SerializeField] private float readyTime = 10f;
    [SerializeField] private float waveDuration = 60f;

    [Header("Countdown")]
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private int countdownSeconds = 3;

    [Header("References")]
    [SerializeField] private SpawnManager spawnManager;
    [SerializeField] private Dessert dessert;

    public float ReadyTime => readyTime;
    public float WaveDuration => waveDuration;
    public float RemainingTime => timer;
    public int CurrentWave { get; private set; }

    private WaveStateMachine stateMachine;
    private float timer;

    private void Awake()
    {
        stateMachine = new WaveStateMachine();
    }

    private void OnEnable()
    {
        UpgradeFlowSignal.OnResolved += HandleUpgradesResolved;
        DessertDestroyedSignal.OnDessertDestroyed += HandleDessertDestroyed;

        AntDeathSignal.OnAntDied += TrackAntDeath;
        SpawnManager.OnAntSpawned += TrackAntSpawned;
        SpawnManager.OnSpawnComplete += TrackSpawnComplete;
    }

    private void OnDisable()
    {
        UpgradeFlowSignal.OnResolved -= HandleUpgradesResolved;
        DessertDestroyedSignal.OnDessertDestroyed -= HandleDessertDestroyed;

        AntDeathSignal.OnAntDied -= TrackAntDeath;
        SpawnManager.OnAntSpawned -= TrackAntSpawned;
        SpawnManager.OnSpawnComplete -= TrackSpawnComplete;
    }
    private void Start()
    {
        CurrentWave = 0;
        StartCoroutine(StartCountdownSequence(countdownSeconds));
    }

    public bool IsPlanning()
    {
        return stateMachine.IsInState<PlanningState>();
    }

    public bool IsPlaying()
    {
        return stateMachine.IsInState<PlayingState>();
    }


    private void Update()
    {
        stateMachine.Update();
    }

    // -------------------------
    // Planning
    // -------------------------

    public void StartPlanningPhase()
    {
        timer = readyTime;

        if (dessert != null)
        {
            dessert.ResetHealth();
        }

        Debug.Log("Planning Phase Started");

        WaveReadySignal.Raise();
    }

    public void UpdatePlanningTimer()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            stateMachine.ChangeState(
                new PlayingState(this)
            );
        }
    }

    // -------------------------
    // Playing
    // -------------------------

    public void StartPlayingPhase()
    {
        CurrentWave++;
        timer = waveDuration;

        Debug.Log($"Wave {CurrentWave} Started");

        if (spawnManager != null)
        {
            spawnManager.StartWave();
        }
    }

    public void UpdatePlayingTimer()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            stateMachine.ChangeState(
                new UpgradeState(this)
            );
        }
    }

    // -------------------------
    // Upgrade
    // -------------------------

    private void HandleUpgradesResolved()
    {
        Debug.Log("=== UPGRADE RESOLVED ===");

        StartCoroutine(StartCountdownSequence(countdownSeconds));
    }

    public void StartUpgradePhase()
    {
        if (spawnManager != null)
        {
            spawnManager.ClearPreviousWave();
        }

        Debug.Log($"Wave {CurrentWave} Completed");

        WaveEndSignal.Raise();
    }


    // -------------------------
    // Game Over
    // -------------------------
    private void HandleDessertDestroyed()
    {
        Debug.Log("Player Lost - Dessert Destroyed!");

        stateMachine.ChangeState(new GameOverState(this));
    }

    public void HandleGameOver()
    {
        Debug.Log("GAME OVER");

        if (spawnManager != null)
        {
            spawnManager.ClearPreviousWave();
        }

        SceneManager.LoadScene("GameOverScene");
    }

    // -------------------------
    // Countdown
    // -------------------------

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

        stateMachine.ChangeState(
            new PlanningState(this)
        );
    }

    public void FinishUpgrade()
    {
        StartCoroutine(
            StartCountdownSequence(countdownSeconds)
        );
    }

    // -------------------------
    // Victory 
    // -------------------------
    private void Victory()
    {
        spawnManager.ClearPreviousWave();
        OnVictory?.Invoke();
    }

    private void CheckVictory()
    {
        if (stateMachine != null && IsPlaying() &&
           isSpawningFinished && activeAnts <= 0)
        {
            Victory();
        }
    }

    private void TrackAntSpawned()
    {
        activeAnts++;
    }
    private void TrackAntDeath(float expValue)
    {
        activeAnts = Mathf.Max(0, activeAnts -1 );
        CheckVictory();
    }
    private void TrackSpawnComplete()
    {
        isSpawningFinished = true;
        CheckVictory();
    }
}





//public class WaveManager : MonoBehaviour
//{
//    [Header("Time Settings")]
//    [SerializeField] private float readyTime = 10f;
//    [SerializeField] private float waveDuration = 60f;

//    //--------------------------------------- (new) count down before each wave start (also handles the first tnt placement when clicking any button)
//    [SerializeField] private TextMeshProUGUI countdownText;
//    [SerializeField] private int countdownSeconds = 3;
//    //----------------------------------------

//    [Header("References")]
//    [SerializeField] private SpawnManager spawnManager;
//    [SerializeField] private Dessert dessert;
//    public float ReadyTime => readyTime;
//    public float WaveDuration => waveDuration;
//    public float RemainingTime => timer;
//    public int CurrentWave { get; private set; }
//    // public WaveState CurrentState { get; private set; }
//    private WaveStateMachine stateMachine;
//    private float timer;

//    private void Awake()
//    {
//        stateMachine = new WaveStateMachine();
//    }
//    private void OnEnable()
//    {
//        // The only thing WaveManager knows about the upgrade flow: "someone
//        // will tell me when it's resolved." It never references
//        // ExperienceManager or UpgradeSelectionUI directly.
//        UpgradeFlowSignal.OnResolved += HandleUpgradesResolved;
//        DessertDestroyedSignal.OnDessertDestroyed += HandleDessertDestroyed;
//    }
//    private void OnDisable()
//    {
//        UpgradeFlowSignal.OnResolved -= HandleUpgradesResolved;
//        DessertDestroyedSignal.OnDessertDestroyed -= HandleDessertDestroyed;
//    }

//    private void Start()
//    {
//        CurrentWave = 0;
//        //CurrentState = WaveState.WaitingToStart;

//        StartCoroutine(StartCountdownSequence(countdownSeconds)); // just start without any buttons to be pressed 

//    }

//    private void Update()
//    {
//        //if (CurrentState == WaveState.GameOver)
//        //    return;

//        //if (CurrentState != WaveState.Ready &&
//        //    CurrentState != WaveState.Playing)
//        //    return;

//        //timer -= Time.deltaTime;

//        //if (timer <= 0f)
//        //{
//        //    HandleTimerFinished();
//        //}
//        stateMachine.Update();
//    }
//    public void UpdatePlanningTimer()
//    {
//        timer -= Time.deltaTime;

//        if (timer <= 0f)
//        {
//            stateMachine.ChangeState(
//                new PlayingState(this)
//            );
//        }
//    }

//    // used in play button 
//    //private void HandleTimerFinished()
//    //{
//    //    switch (CurrentState)
//    //    {
//    //        case WaveState.Ready:
//    //            StartWave();
//    //            break;

//    //        case WaveState.Playing:
//    //            EndWave();
//    //            break;
//    //    }
//    //}

//    // Called by Start Button
//    //public void StartPlanning()
//    //{
//    //    spawnManager.ClearPreviousWave();
//    //    if (CurrentState != WaveState.WaitingToStart)
//    //        return;

//    //    //StartReadyPhase();
//    //    StartCoroutine(StartCountdownSequence(countdownSeconds));
//    //}
//    public void StartPlanningPhase()
//    {
//        timer = readyTime;

//        if (dessert != null)
//        {
//            dessert.ResetHealth();
//        }

//        Debug.Log("Planning Phase Started");

//        WaveReadySignal.Raise();
//    }
//    public void StartPlayingPhase()
//    {
//        CurrentWave++;

//        timer = waveDuration;

//        Debug.Log($"Wave {CurrentWave} Started");

//        if (spawnManager != null)
//        {
//            spawnManager.StartWave();
//        }
//    }
//    private void StartReadyPhase()
//    {
//        CurrentState = WaveState.Ready;
//        timer = readyTime;
//        if (dessert != null)
//        {
//            dessert.ResetHealth();
//        }
//        Debug.Log("Planning Phase Started");

//        WaveReadySignal.Raise();
//    }

//    private void StartWave()
//    {

//        CurrentWave++;

//        CurrentState = WaveState.Playing;
//        timer = waveDuration;

//        Debug.Log($"Wave {CurrentWave} Started");

//        if (spawnManager != null)
//        {
//            spawnManager.StartWave();
//        }
//    }

//    private void EndWave()
//    {
//        spawnManager.ClearPreviousWave();
//        CurrentState = WaveState.Upgrade;

//        Debug.Log($"Wave {CurrentWave} Completed");

//        // Whether the player leveled up zero times or five times this wave
//        // is none of WaveManager's business - it just announces the wave
//        // is over and waits to be told it can continue.
//        WaveEndSignal.Raise();
//    }

//    // Fires when ExperienceManager (nothing pending) or UpgradeSelectionUI
//    // (player finished picking) says the post-wave upgrade step is done.
//    private void HandleUpgradesResolved()
//    {
//        if (CurrentState != WaveState.Upgrade)
//            return; // ignore stray/duplicate signals outside the Upgrade state

//        //StartReadyPhase();
//        StartCoroutine(StartCountdownSequence(countdownSeconds));
//    }

//    private IEnumerator StartCountdownSequence(int num)
//    {
//        countdownText.gameObject.SetActive(true);
//        for (int i = num; i > 0; i--)
//        {
//            if (countdownText != null)
//            {
//                countdownText.text = i.ToString();
//            }
//            yield return new WaitForSeconds(1f);
//        }
//        countdownText.text = "START!";
//        yield return new WaitForSeconds(0.5f);
//        countdownText.gameObject.SetActive(false);
//        StartReadyPhase();
//    }

//    public void FinishUpgrade()
//    {
//        //StartReadyPhase();
//        StartCoroutine(StartCountdownSequence(countdownSeconds));
//    }
//    private void HandleDessertDestroyed()
//    {
//        Debug.Log("Player Lost - Dessert Destroyed!");

//        stateMachine.ChangeState(
//            new GameOverState(this)
//        );
//    }
//    //private void HandleDessertDestroyed()
//    //{
//    //    if (CurrentState != WaveState.Playing)
//    //        return;

//    //    Debug.Log("Player Lost - Dessert Destroyed!");

//    //    if (spawnManager != null)
//    //    {
//    //        spawnManager.ClearPreviousWave();
//    //    }

//    //    GameOver();
//    //}
//    public void StartUpgradePhase()
//    {
//        if (spawnManager != null)
//        {
//            spawnManager.ClearPreviousWave();
//        }

//        Debug.Log($"Wave {CurrentWave} Completed");

//        WaveEndSignal.Raise();
//    }
//    public void HandleGameOver()
//    {
//        if (spawnManager != null)
//        {
//            spawnManager.ClearPreviousWave();
//        }

//        Debug.Log("GAME OVER");

//        // Later:
//        // SceneManager.LoadScene("GameOverScene");
//    }
//    public void GameOver()
//    {
//        //CurrentState = WaveState.GameOver;

//        Debug.Log("GAME OVER");

//        // Later: Show Game Over Screen
//    }
//}