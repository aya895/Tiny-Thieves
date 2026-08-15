using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static event Action OnVictory;
    public static event Action OnWaveReady;
    public static event Action OnWaveEnded;

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
    [SerializeField] private VictoryTracker victoryTracker;

    private WaveStateMachine stateMachine;

    public int CurrentWave { get; private set; }

    public float ReadyTime => readyTime;
    public float WaveDuration => waveDuration;
    public float RemainingTime => timer;

    private float timer;

    private void Awake()
    {
        Time.timeScale = 1f;

        stateMachine = new WaveStateMachine();

        if (victoryTracker == null)
        {
            victoryTracker = GetComponent<VictoryTracker>();
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
    }

    private void Start()
    {
        CurrentWave = 0;

        StartCoroutine(
            StartCountdownSequence(countdownSeconds)
        );
    }

    private void Update()
    {
        stateMachine.Update();
    }

    // =========================================================
    // STATE QUERIES
    // =========================================================

    public bool IsPlanning()
    {
        return stateMachine.IsInState<PlanningState>();
    }

    public bool IsPlaying()
    {
        return stateMachine.IsInState<PlayingState>();
    }

    public bool IsUpgrading()
    {
        return stateMachine.IsInState<UpgradeState>();
    }

    public bool IsGameOver()
    {
        return stateMachine.IsInState<GameOverState>();
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
        CurrentWave++;

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
        StartCoroutine(
            StartCountdownSequence(countdownSeconds)
        );
    }

    // =========================================================
    // GAME OVER STATE
    // =========================================================

    private void HandleDessertDestroyed()
    {
        if (IsGameOver())
            return;

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

        if (experienceManager != null)
        {
            experienceManager.ResetProgress();
        }

        if (gameOverUI != null)
        {
            gameOverUI.Show();
        }
    }

    public void ContinueAfterGameOver()
    {
        StartCoroutine(
            StartCountdownSequence(countdownSeconds)
        );
    }

    // =========================================================
    // COUNTDOWN
    // =========================================================

    private IEnumerator StartCountdownSequence(int seconds)
    {
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(true);
        }

        for (int i = seconds; i > 0; i--)
        {
            if (countdownText != null)
            {
                countdownText.text = i.ToString();
            }

            yield return new WaitForSeconds(1f);
        }

        if (countdownText != null)
        {
            countdownText.text = "START!";
        }

        yield return new WaitForSeconds(0.5f);

        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(false);
        }

        stateMachine.ChangeState(
            new PlanningState(this)
        );
    }
}