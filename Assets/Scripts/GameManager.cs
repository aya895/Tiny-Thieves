using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameManager instance { set; get; }
    [SerializeField] private WaveManager waveManager;
    [SerializeField] private ExperienceManager experienceManager;
    private int processedWave = 0;

    //// needed events
    public static event Action OnMoreAntSpeed;
    public static event Action OnNewAntType;
    public static event Action OnAddAntNest;
    public static event Action OnMapExpand;
    public static event Action OnMoreAntInLine;
    //// what more to add??

    [Header("Difficulty Settings")]
    [SerializeField] private int enemyUpgradeInterval = 3;
    [SerializeField] private int mapExpansionInterval = 3;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (waveManager != null)
        {
            int currentWave = waveManager.CurrentWave;
            if (currentWave > 0 && currentWave != processedWave)
            {
                processedWave = currentWave;

                // prime numers so no big conflicts :)
                if (currentWave % enemyUpgradeInterval == 0)
                {
                    OnMoreAntSpeed?.Invoke();
                    OnNewAntType?.Invoke();
                    OnAddAntNest?.Invoke();
                }

                if (currentWave % mapExpansionInterval == 0)
                {
                    OnMapExpand?.Invoke();
                    OnMoreAntInLine?.Invoke();
                }
            }
        }
    }
}
