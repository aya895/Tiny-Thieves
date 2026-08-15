using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private WaveManager waveManager;

    [Header("Difficulty Settings")]
    [SerializeField] private int enemyUpgradeInterval = 3;
    [SerializeField] private int mapExpansionInterval = 5;

    [SerializeField] private float startingWaveDuration = 20f;
    [SerializeField] private float waveDurationIncrement = 5f;

    private int processedWave = 0;

    // Difficulty events
    public static event Action OnMoreAntSpeed;
    public static event Action OnNewAntType;
    public static event Action OnAddAntNest;
    public static event Action OnMapExpand;
    public static event Action OnMoreAntInLine;

    private void Awake()
    {
        // Every Game Scene gets its own GameManager.
        if (FindObjectsByType<GameManager>(FindObjectsInactive.Exclude,FindObjectsSortMode.None).Length > 1)
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        if (waveManager == null)
        {
            waveManager = FindFirstObjectByType<WaveManager>();
        }

        if (waveManager != null)
        {
            waveManager.SetWaveDuration(startingWaveDuration);
        }
    }

    private void Update()
    {
        if (waveManager == null)
        {
            waveManager = FindFirstObjectByType<WaveManager>();
            return;
        }

        int currentWave = waveManager.CurrentWave;

        if (currentWave <= 0 || currentWave == processedWave)
            return;

        processedWave = currentWave;

        // Every 3 waves
        if (currentWave % enemyUpgradeInterval == 0)
        {
            OnMoreAntSpeed?.Invoke();
            OnNewAntType?.Invoke();
            OnAddAntNest?.Invoke();

            int increments = currentWave / enemyUpgradeInterval;
            float newWaveDuration = startingWaveDuration + waveDurationIncrement * increments;
            waveManager.SetWaveDuration(newWaveDuration);
        }

        // Every 5 waves
        if (currentWave % mapExpansionInterval == 0)
        {
            OnMapExpand?.Invoke();
            OnMoreAntInLine?.Invoke();
        }
    }
}