using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    // =========================================================
    // EVENTS
    // =========================================================

    public static event Action OnVictory;
    public static event Action OnWaveReady;
    public static event Action OnWaveEnded;
    public static event Action<IWaveState> OnStateChanged;


    // =========================================================
    // SETTINGS
    // =========================================================

    [Header("Time Settings")]
    [SerializeField] private float readyTime = 10f;
    [SerializeField] private float waveDuration = 60f;

    [Header("Start Message")]
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private string startMessage = "GO!";
    [SerializeField] private float startMessageDuration = 0.75f;


    // =========================================================
    // REFERENCES
    // =========================================================

    [Header("References")]
    [SerializeField] private SpawnManager spawnManager;
    [SerializeField] private Dessert dessert;
    [SerializeField] private GameOverUI gameOverUI;
    [SerializeField] private ExperienceManager experienceManager;
    [SerializeField] private VictoryTracker victoryTracker;


    // =========================================================
    // STATE
    // =========================================================

    private WaveStateMachine stateMachine;
    private float timer;

    private bool retryCurrentWave;

    public int CurrentWave { get; private set; }

    public float ReadyTime => readyTime;
    public float WaveDuration => waveDuration;
    public float RemainingTime => timer;


    // =========================================================
    // UNITY
    // =========================================================

    private void Awake()
    {
        Time.timeScale = 1f;

        stateMachine = new WaveStateMachine();

        stateMachine.OnStateChanged += HandleStateChanged;

        if (victoryTracker == null)
        {
            victoryTracker = GetComponent<VictoryTracker>();
        }

        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        DessertDestroyedSignal.OnDessertDestroyed += HandleDessertDestroyed;

        VictoryTracker.OnVictoryAchieved += HandleVictory;

        if (experienceManager != null)
        {
            experienceManager.UpgradesResolved += HandleUpgradesResolved;
        }
    }

    private void OnDisable()
    {
        DessertDestroyedSignal.OnDessertDestroyed -= HandleDessertDestroyed;

        VictoryTracker.OnVictoryAchieved -= HandleVictory;

        if (experienceManager != null)
        {
            experienceManager.UpgradesResolved -= HandleUpgradesResolved;
        }

        if (stateMachine != null)
        {
            stateMachine.OnStateChanged -= HandleStateChanged;
        }
    }

    private void Start()
    {
        CurrentWave = 0;
        retryCurrentWave = false;

        StartCoroutine(ShowStartMessage());
    }

    private void Update()
    {
        stateMachine?.Update();
    }


    // =========================================================
    // STATE EVENTS
    // =========================================================

    private void HandleStateChanged(IWaveState state)
    {
        OnStateChanged?.Invoke(state);
    }


    // =========================================================
    // STATE QUERIES
    // =========================================================

    public bool IsPlanning()
    {
        return stateMachine != null &&
               stateMachine.IsInState<PlanningState>();
    }

    public bool IsPlaying()
    {
        return stateMachine != null &&
               stateMachine.IsInState<PlayingState>();
    }

    public bool IsUpgrading()
    {
        return stateMachine != null &&
               stateMachine.IsInState<UpgradeState>();
    }

    public bool IsGameOver()
    {
        return stateMachine != null &&
               stateMachine.IsInState<GameOverState>();
    }


    // =========================================================
    // PLANNING STATE
    // =========================================================

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
            stateMachine.ChangeState(
                new PlayingState(this)
            );
        }
    }


    // =========================================================
    // PLAYING STATE
    // =========================================================

    public void StartPlayingPhase()
    {
        if (retryCurrentWave)
        {
            retryCurrentWave = false;
        }
        else
        {
            CurrentWave++;
        }

        timer = waveDuration;

        if (victoryTracker != null)
        {
            victoryTracker.Reset();
        }

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
            FinishWave();
        }
    }

    private void FinishWave()
    {
        if (!IsPlaying())
            return;

        stateMachine.ChangeState(
            new UpgradeState(this)
        );
    }


    // =========================================================
    // VICTORY
    // =========================================================

    private void HandleVictory()
    {
        if (!IsPlaying())
            return;

        retryCurrentWave = false;

        OnVictory?.Invoke();

        FinishWave();
    }


    // =========================================================
    // UPGRADE STATE
    // =========================================================

    public void StartUpgradePhase()
    {
        if (spawnManager != null)
        {
            spawnManager.ClearPreviousWave();
        }

        OnWaveEnded?.Invoke();

        if (experienceManager != null)
        {
            experienceManager.ResolveWaveEnd();
            return;
        }

        HandleUpgradesResolved();
    }

    private void HandleUpgradesResolved()
    {
        StartCoroutine(ShowStartMessage());
    }


    // =========================================================
    // GAME OVER
    // =========================================================

    private void HandleDessertDestroyed()
    {
        if (!IsPlaying())
            return;

        // Player earned at least one upgrade during this wave.
        // Even though the dessert was destroyed,
        // allow progression to the next wave.
        if (experienceManager != null &&
            experienceManager.PendingLevelUps > 0)
        {
            retryCurrentWave = false;

            stateMachine.ChangeState(
                new UpgradeState(this)
            );

            return;
        }

        // No upgrade was earned.
        // The player must retry the same wave.
        retryCurrentWave = true;

        stateMachine.ChangeState(
            new GameOverState(this)
        );
    }

    public void HandleGameOver()
    {
        if (spawnManager != null)
        {
            spawnManager.ClearPreviousWave();
        }

        OnWaveEnded?.Invoke();

        if (gameOverUI != null)
        {
            gameOverUI.Show();
        }
    }

    public void ContinueAfterGameOver()
    {
        if (experienceManager != null)
        {
            experienceManager.ResolveWaveEnd();
            return;
        }

        StartCoroutine(ShowStartMessage());
    }


    // =========================================================
    // START MESSAGE
    // =========================================================

    private IEnumerator ShowStartMessage()
    {
        if (countdownText != null)
        {
            countdownText.text = startMessage;
            countdownText.gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(
            startMessageDuration
        );

        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(false);
        }

        stateMachine.ChangeState(
            new PlanningState(this)
        );
    }
}