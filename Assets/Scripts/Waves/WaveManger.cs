using System;
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
    [SerializeField] private float waveDuration = 20f;


    // =========================================================
    // REFERENCES
    // =========================================================

    
    [Header("References")]
    [SerializeField] private SpawnManager spawnManager;
    [SerializeField] private Dessert dessert;
    [SerializeField] private ExperienceManager experienceManager;
    [SerializeField] private VictoryTracker victoryTracker;
    [SerializeField] private WaveCountdownController countdownController;

    // =========================================================
    // STATE
    // =========================================================

    private WaveStateMachine stateMachine;

    private bool retryCurrentWave;
    private bool waveEndLocked;


    // =========================================================
    // PROPERTIES
    // =========================================================

    public int CurrentWave { get; private set; }

    public float ReadyTime => readyTime;

    public float WaveDuration => waveDuration;

    public bool IsWaveEndLocked => waveEndLocked;


    public float RemainingTime
    {
        get
        {
            if (stateMachine.CurrentState is PlanningState planning)
                return planning.RemainingTime;

            if (stateMachine.CurrentState is PlayingState playing)
                return playing.RemainingTime;

            return 0f;
        }
    }


    // =========================================================
    // UNITY
    // =========================================================

    private void Awake()
    {
        stateMachine = new WaveStateMachine();

        stateMachine.OnStateChanged +=
            HandleStateChanged;

        if (victoryTracker == null)
        {
            victoryTracker =
                GetComponent<VictoryTracker>();
        }
    }

    private void Start()
    {
        CurrentWave = 0;

        retryCurrentWave = false;

        waveEndLocked = false;

        ChangeState(
            new PlanningState(this)
        );
    }
    private void OnEnable()
    {
        DessertDestroyedSignal.OnDessertDestroyed +=
            HandleDessertDestroyed;

        VictoryTracker.OnVictoryAchieved +=
            HandleVictory;

        WaveCountdownController.OnCountdownFinished +=
            HandleCountdownFinished;

        if (experienceManager != null)
        {
            experienceManager.UpgradesResolved +=
                HandleUpgradesResolved;
        }
    }


    private void OnDisable()
    {
        DessertDestroyedSignal.OnDessertDestroyed -=
            HandleDessertDestroyed;

        VictoryTracker.OnVictoryAchieved -=
            HandleVictory;

        WaveCountdownController.OnCountdownFinished -=
            HandleCountdownFinished;

        if (experienceManager != null)
        {
            experienceManager.UpgradesResolved -=
                HandleUpgradesResolved;
        }

        if (stateMachine != null)
        {
            stateMachine.OnStateChanged -=
                HandleStateChanged;
        }
    }

    
    private void StartCountdown()
    {
        if (countdownController != null)
        {
            countdownController.StartCountdown();
            return;
        }

        HandleCountdownFinished();
    }


    private void HandleCountdownFinished()
    {
        ChangeState(
            new PlanningState(this)
        );
    }


    private void Update()
    {
        stateMachine.Update();
    }


    // =========================================================
    // STATE MACHINE
    // =========================================================

    public void ChangeState(IWaveState state)
    {
        stateMachine.ChangeState(state);
    }


    private void HandleStateChanged(IWaveState state)
    {
        OnStateChanged?.Invoke(state);
    }


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
    // PLANNING
    // =========================================================

    public void PrepareWave()
    {
        if (dessert != null)
        {
            dessert.ResetHealth();
        }
    }


    public void NotifyWaveReady()
    {
        OnWaveReady?.Invoke();
    }


    // =========================================================
    // PLAYING
    // =========================================================

    public void BeginWave()
    {
        waveEndLocked = false;

        if (retryCurrentWave)
        {
            retryCurrentWave = false;
        }
        else
        {
            CurrentWave++;
        }


        if (victoryTracker != null)
        {
            victoryTracker.Reset();
        }


        if (spawnManager != null)
        {
            spawnManager.StartWave();
        }
    }


    public void CompleteWave()
    {
        if (!TryLockWaveEnd())
            return;

        ChangeState(
            new UpgradeState(this)
        );
    }


    // =========================================================
    // VICTORY
    // =========================================================

    private void HandleVictory()
    {
        if (!TryLockWaveEnd())
            return;

        retryCurrentWave = false;

        OnVictory?.Invoke();

        ChangeState(
            new UpgradeState(this)
        );
    }


    // =========================================================
    // DESSERT DESTROYED
    // =========================================================

    private void HandleDessertDestroyed()
    {
        if (!TryLockWaveEnd())
            return;


        if (experienceManager != null &&
            experienceManager.PendingLevelUps > 0)
        {
            retryCurrentWave = false;

            ChangeState(
                new UpgradeState(this)
            );

            return;
        }


        retryCurrentWave = true;

        ChangeState(
            new GameOverState(this)
        );
    }


    // =========================================================
    // UPGRADE
    // =========================================================

    public void BeginUpgradePhase()
    {
        ClearWave();

        if (experienceManager != null)
        {
            experienceManager.ResolveWaveEnd();

            return;
        }

        StartNextWave();
    }


    private void HandleUpgradesResolved()
    {
        StartNextWave();
    }


    // =========================================================
    // GAME OVER
    // =========================================================

    public void BeginGameOver()
    {
        ClearWave();
    }


    public void ContinueAfterGameOver()
    {
        if (experienceManager != null)
        {
            experienceManager.ResolveWaveEnd();

            return;
        }

        StartNextWave();
    }


    // =========================================================
    // WAVE CLEANUP
    // =========================================================

    private void ClearWave()
    {
        if (spawnManager != null)
        {
            spawnManager.ClearPreviousWave();
        }

        OnWaveEnded?.Invoke();
    }


    // =========================================================
    // WAVE END PROTECTION
    // =========================================================

    private bool TryLockWaveEnd()
    {
        if (!IsPlaying())
            return false;

        if (waveEndLocked)
            return false;

        waveEndLocked = true;

        return true;
    }


    // =========================================================
    // NEXT WAVE
    // =========================================================

    


    // =========================================================
    // SETTINGS
    // =========================================================

    public void SetWaveDuration(float duration)
    {
        waveDuration = duration;
    }
    private void StartNextWave()
    {
        StartCountdown();
    }
}