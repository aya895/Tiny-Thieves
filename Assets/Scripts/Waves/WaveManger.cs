//using System.Collections;
//using TMPro;
//using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
public class WaveManager : MonoBehaviour
{
<<<<<<< Updated upstream
    
=======
    // =========================================================
    // EVENTS
    // =========================================================

    public static event Action OnVictory;
    public static event Action OnWaveReady;
    public static event Action OnWaveEnded;
    public static event Action<IWaveState> OnStateChanged;
    public static event Action OnCountdownStarted;


    // =========================================================
    // SETTINGS
    // =========================================================

>>>>>>> Stashed changes
    [Header("Time Settings")]
    [SerializeField] private float readyTime = 10f;
    [SerializeField] private float waveDuration = 60f;

<<<<<<< Updated upstream
    [Header("Countdown")]
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private int countdownSeconds = 3;
=======
    [Header("Start Countdown")]
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private int countdownStartNumber = 3;

    private const float CountdownStepDuration = 1f;
    private const float GoDuration = 0.6f;


    // =========================================================
    // REFERENCES
    // =========================================================
>>>>>>> Stashed changes

    
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

<<<<<<< Updated upstream
=======
    private bool retryCurrentWave;

    // Prevents multiple wave-ending events
    // from running in the same frame.
    private bool waveEndLocked;

    public int CurrentWave { get; private set; }

    public float ReadyTime => readyTime;
    public float WaveDuration => waveDuration;
    public float RemainingTime => timer;


    // =========================================================
    // UNITY
    // =========================================================

>>>>>>> Stashed changes
    private void Awake()
    {
        stateMachine = new WaveStateMachine();
<<<<<<< Updated upstream
=======

        stateMachine.OnStateChanged +=
            HandleStateChanged;

        if (victoryTracker == null)
        {
            victoryTracker =
                GetComponent<VictoryTracker>();
        }

        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(false);
        }
>>>>>>> Stashed changes
    }

    private void OnEnable()
    {
<<<<<<< Updated upstream
        UpgradeFlowSignal.OnResolved += HandleUpgradesResolved;
        DessertDestroyedSignal.OnDessertDestroyed += HandleDessertDestroyed;
=======
        DessertDestroyedSignal.OnDessertDestroyed +=
            HandleDessertDestroyed;

        VictoryTracker.OnVictoryAchieved +=
            HandleVictory;

        if (experienceManager != null)
        {
            experienceManager.UpgradesResolved +=
                HandleUpgradesResolved;
        }
>>>>>>> Stashed changes
    }

    private void OnDisable()
    {
<<<<<<< Updated upstream
        UpgradeFlowSignal.OnResolved -= HandleUpgradesResolved;
        DessertDestroyedSignal.OnDessertDestroyed -= HandleDessertDestroyed;
    }
    public bool IsPlanning()
    {
        return stateMachine.IsInState<PlanningState>();
=======
        DessertDestroyedSignal.OnDessertDestroyed -=
            HandleDessertDestroyed;

        VictoryTracker.OnVictoryAchieved -=
            HandleVictory;

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

    private void Start()
    {
        CurrentWave = 0;

        retryCurrentWave = false;
        waveEndLocked = false;

        StartCoroutine(
            ShowStartCountdown()
        );
    }

    private void Update()
    {
        stateMachine?.Update();
    }


    // =========================================================
    // STATE EVENTS
    // =========================================================

    private void HandleStateChanged(
        IWaveState state)
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
>>>>>>> Stashed changes
    }

    public bool IsPlaying()
    {
<<<<<<< Updated upstream
        return stateMachine.IsInState<PlayingState>();
=======
        return stateMachine != null &&
               stateMachine.IsInState<PlayingState>();
>>>>>>> Stashed changes
    }
    private void Start()
    {
<<<<<<< Updated upstream
        CurrentWave = 0;

        StartCoroutine(
            StartCountdownSequence(countdownSeconds)
        );
=======
        return stateMachine != null &&
               stateMachine.IsInState<UpgradeState>();
>>>>>>> Stashed changes
    }

    private void Update()
    {
<<<<<<< Updated upstream
        stateMachine.Update();
=======
        return stateMachine != null &&
               stateMachine.IsInState<GameOverState>();
    }


    // =========================================================
    // WAVE SETTINGS
    // =========================================================

    public void SetWaveDuration(float duration)
    {
        waveDuration = duration;
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
        CurrentWave++;
=======
        // New wave starts,
        // so wave-ending events are allowed again.
        waveEndLocked = false;

        if (retryCurrentWave)
        {
            retryCurrentWave = false;
        }
        else
        {
            CurrentWave++;
        }

>>>>>>> Stashed changes
        timer = waveDuration;

        Debug.Log($"Wave {CurrentWave} Started");

        if (spawnManager != null)
        {
            spawnManager.StartWave();
        }
    }

    public void UpdatePlayingTimer()
    {
        if (waveEndLocked)
            return;

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            stateMachine.ChangeState(
                new UpgradeState(this)
            );
        }
    }

<<<<<<< Updated upstream
    // -------------------------
    // Upgrade
    // -------------------------
=======
    private void FinishWave()
    {
        if (!IsPlaying() ||
            waveEndLocked)
        {
            return;
        }

        waveEndLocked = true;
>>>>>>> Stashed changes

    private void HandleUpgradesResolved()
    {
        Debug.Log("=== UPGRADE RESOLVED ===");

        StartCoroutine(
            StartCountdownSequence(countdownSeconds)
        );
    }

<<<<<<< Updated upstream
=======

    // =========================================================
    // VICTORY
    // =========================================================

    private void HandleVictory()
    {
        if (!IsPlaying() ||
            waveEndLocked)
        {
            return;
        }

        waveEndLocked = true;

        retryCurrentWave = false;

        OnVictory?.Invoke();

        stateMachine.ChangeState(
            new UpgradeState(this)
        );
    }


    // =========================================================
    // UPGRADE STATE
    // =========================================================

>>>>>>> Stashed changes
    public void StartUpgradePhase()
    {
        if (spawnManager != null)
        {
            spawnManager.ClearPreviousWave();
        }

<<<<<<< Updated upstream
        Debug.Log($"Wave {CurrentWave} Completed");

        WaveEndSignal.Raise();
=======
        OnWaveEnded?.Invoke();
    }

    public void ContinueAfterVictory()
    {
        if (experienceManager != null)
        {
            experienceManager.ResolveWaveEnd();
            return;
        }

        StartCoroutine(
            ShowStartCountdown()
        );
    }

    private void HandleUpgradesResolved()
    {
        StartCoroutine(
            ShowStartCountdown()
        );
>>>>>>> Stashed changes
    }


    // -------------------------
    // Game Over
    // -------------------------

    private void HandleDessertDestroyed()
    {
<<<<<<< Updated upstream
        Debug.Log("Player Lost - Dessert Destroyed!");

=======
        if (!IsPlaying() ||
            waveEndLocked)
        {
            return;
        }

        waveEndLocked = true;

        if (experienceManager != null &&
            experienceManager.PendingLevelUps > 0)
        {
            retryCurrentWave = false;

            stateMachine.ChangeState(
                new UpgradeState(this)
            );

            return;
        }

        retryCurrentWave = true;

>>>>>>> Stashed changes
        stateMachine.ChangeState(
            new GameOverState(this)
        );
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
<<<<<<< Updated upstream
            StartCountdownSequence(countdownSeconds)
=======
            ShowStartCountdown()
>>>>>>> Stashed changes
        );
    }

    // -------------------------
    // Countdown
    // -------------------------

<<<<<<< Updated upstream
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

=======
    // =========================================================
    // START COUNTDOWN
    // =========================================================

    private IEnumerator ShowStartCountdown()
    {
        OnCountdownStarted?.Invoke();

        if (countdownText == null)
        {
            stateMachine.ChangeState(
                new PlanningState(this)
            );

            yield break;
        }

        countdownText.gameObject.SetActive(true);

        for (
            int number = countdownStartNumber;
            number > 0;
            number--)
        {
            countdownText.text =
                number.ToString();

            yield return new WaitForSeconds(
                CountdownStepDuration
            );
        }

        countdownText.text = "GO!";

        yield return new WaitForSeconds(
            GoDuration
        );

>>>>>>> Stashed changes
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
}
