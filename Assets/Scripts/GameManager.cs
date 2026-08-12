using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameManager instance { set; get; }
    [SerializeField] private WaveManager waveManager;
    [SerializeField] private ExperienceManager experienceManager;

    //// needed events
    public static event Action OnMoreAntSpeed;
    public static event Action OnNewAntType;
    public static event Action OnAddAntNest;
    public static event Action OnMapExpand; //still in progress
    public static event Action OnMoreAntInLine;

    //// what more to add??
    private int processedWave = 0;

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
                if (currentWave % 3 == 0)
                {
                    OnMoreAntSpeed?.Invoke();
                    OnNewAntType?.Invoke();
                    OnAddAntNest?.Invoke();
                }

                if (currentWave % 7 == 0)
                {
                    OnMapExpand?.Invoke();
                    OnMoreAntInLine?.Invoke();
                }
            }
        }
    }
}
