using System;
using UnityEngine;

public class VictoryTracker : MonoBehaviour
{
    private int activeAnts = 0;
    private bool isSpawningFinished = false;

    public static event Action OnVictoryAchieved;

    private void OnEnable()
    {
        Ant.OnAntDeath += OnAntDeath;
        SpawnManager.OnAntSpawned += OnAntSpawned;
        SpawnManager.OnSpawnComplete += OnSpawnComplete;
    }

    private void OnDisable()
    {
        Ant.OnAntDeath -= OnAntDeath;
        SpawnManager.OnAntSpawned -= OnAntSpawned;
        SpawnManager.OnSpawnComplete -= OnSpawnComplete;
    }

    public void Reset()
    {
        activeAnts = 0;
        isSpawningFinished = false;
    }

    public void OnAntSpawned()
    {
        activeAnts++;
    }

    public void OnAntDeath(GameObject ant, float expValue)
    {
        activeAnts = Mathf.Max(0, activeAnts - 1);
        CheckVictory();
    }

    public void OnSpawnComplete()
    {
        isSpawningFinished = true;
        CheckVictory();
    }

    private void CheckVictory()
    {
        if (isSpawningFinished && activeAnts <= 0)
        {
            OnVictoryAchieved?.Invoke();
        }
    }
}
