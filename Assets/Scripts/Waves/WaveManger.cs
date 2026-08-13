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

    [SerializeField] private GameOverUI gameOverUI;
    [SerializeField] private ExperienceManager experienceManager;
    [SerializeField] private UpgradeSelectionUI upgradeSelectionUI;

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

        gameOverUI.Show();
    }
    public void ContinueAfterGameOver()
    {
        if (experienceManager.PendingLevelUps > 0)
        {
            upgradeSelectionUI.ShowUpgrade();
            return;
        }

        StartCoroutine(
            StartCountdownSequence(countdownSeconds)
        );
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