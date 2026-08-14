using System;
using System.Collections;
using TMPro;
using UnityEngine;
public class WaveManager : MonoBehaviour
{

    public static event Action OnVictory;
    public static event Action OnWaveReady;
    public static event Action OnWaveEnded;

    //[Header("Wave Victory Check")]
    //private int activeAnts = 0;
    //private bool isSpawningFinished = false;

    [Header("Time Settings")]
    [SerializeField] private float readyTime = 10f;
    [SerializeField] private float waveDuration = 60f;

    [Header("Countdown")]
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private int countdownSeconds = 3;

    [Header("References")]
    [SerializeField] private SpawnManager spawnManager;
    [SerializeField] private Dessert dessert;
    [SerializeField] private GameOverUI gameOverUI;
    [SerializeField] private ExperienceManager experienceManager;
    [SerializeField] private UpgradeSelectionUI upgradeSelectionUI;

    private WaveStateMachine stateMachine;
    private VictoryTracker victoryTracker;
    public int CurrentWave { get; private set; }
    public float ReadyTime => readyTime;
    public float WaveDuration => waveDuration;
    public float RemainingTime => timer;
    private float timer;

    private void Awake()
    {
        Time.timeScale = 1f;

        stateMachine = new WaveStateMachine();
        victoryTracker = new VictoryTracker();
    }

    private void OnEnable()
    {
        UpgradeFlowSignal.OnResolved += HandleUpgradesResolved;
        DessertDestroyedSignal.OnDessertDestroyed += HandleDessertDestroyed;

        //Ant.OnAntDeath += TrackAntDeath;
        //SpawnManager.OnAntSpawned += TrackAntSpawned;
        //SpawnManager.OnSpawnComplete += TrackSpawnComplete;
        VictoryTracker.OnVictoryAchieved += HandleVictory;
    }

    private void OnDisable()
    {
        UpgradeFlowSignal.OnResolved -= HandleUpgradesResolved;
        DessertDestroyedSignal.OnDessertDestroyed -= HandleDessertDestroyed;

        //Ant.OnAntDeath -= TrackAntDeath;
        //SpawnManager.OnAntSpawned -= TrackAntSpawned;
        //SpawnManager.OnSpawnComplete -= TrackSpawnComplete;
        VictoryTracker.OnVictoryAchieved -= HandleVictory;
    }
    private void Start()
    {
        CurrentWave = 0;
        StartCoroutine(StartCountdownSequence(countdownSeconds));
    }
    private void Update()
    {
        stateMachine.Update();
    }

    public bool IsPlanning()
    {
        return stateMachine.IsInState<PlanningState>();
    }

    public bool IsPlaying()
    {
        return stateMachine.IsInState<PlayingState>();
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
        OnWaveReady?.Invoke();
    }

    public void UpdatePlanningTimer()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            stateMachine.ChangeState(new PlayingState(this));
        }
    }

    public void StartPlayingPhase()
    {
        CurrentWave++;
        timer = waveDuration;
        victoryTracker.Reset();

        // Reset victory tracking for the new wave
        //activeAnts = 0;
        //isSpawningFinished = false;
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
            stateMachine.ChangeState(new UpgradeState(this));
        }
    }

    // -------------------------
    // Upgrade
    // -------------------------
    public void StartUpgradePhase()
    {
        if (spawnManager != null)
        {
            spawnManager.ClearPreviousWave();
        }
        OnWaveEnded?.Invoke();
    }

    private void HandleUpgradesResolved()
    {
        StartCoroutine(StartCountdownSequence(countdownSeconds));
    }


    // -------------------------
    // Game Over
    // -------------------------
    private void HandleDessertDestroyed()
    {
        stateMachine.ChangeState(new GameOverState(this));
    }

    public void HandleGameOver()
    {
        if (spawnManager != null)
        {
            spawnManager.ClearPreviousWave();
        }
        if (gameOverUI != null)
        {
            gameOverUI.Show();
        }
    }

    public void ContinueAfterGameOver()
    {
        if (experienceManager.PendingLevelUps > 0)
        {
            upgradeSelectionUI.ShowUpgrade();
            return;
        }

        StartCoroutine(StartCountdownSequence(countdownSeconds));
    }

    // -------------------------
    // Countdown
    // -------------------------

    private IEnumerator StartCountdownSequence(int num)
    {
        countdownText?.gameObject.SetActive(true);

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
        stateMachine?.ChangeState(new PlanningState(this));
    }

    public void FinishUpgrade()
    {
        StartCoroutine(StartCountdownSequence(countdownSeconds));
    }

    // -------------------------
    // Victory 
    // -------------------------
    private void HandleVictory()
    {
        if (IsPlaying())
        {
            if (spawnManager != null) spawnManager.ClearPreviousWave();
            OnVictory?.Invoke();
        }
    }




    //private void Victory()
    //{
    //    spawnManager.ClearPreviousWave();
    //    OnVictory?.Invoke();
    //}

    //private void CheckVictory()
    //{
    //    if (stateMachine != null && IsPlaying() &&
    //       isSpawningFinished && activeAnts <= 0)
    //    {
    //        Victory();
    //    }
    //}

    //private void TrackAntSpawned()
    //{
    //    activeAnts++;
    //}
    //private void TrackAntDeath(GameObject ant, float expValue)
    //{
    //    activeAnts = Mathf.Max(0, activeAnts -1 );
    //    CheckVictory();
    //}
    //private void TrackSpawnComplete()
    //{
    //    isSpawningFinished = true;
    //    CheckVictory();
    //}
}