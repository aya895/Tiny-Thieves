using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("Nest")]
    [SerializeField] private GameObject antNest;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnDelay = 0.75f;

    [SerializeField] private float xMin = -8f;
    [SerializeField] private float xMax = -6f;
    [SerializeField] private float yMin = -4f;
    [SerializeField] private float yMax = 1f;

    [SerializeField] private float distancePerNest = 5f;

    [Header("Dessert Distance")]
    [SerializeField] private Transform dessertTransform;
    [SerializeField] private float minDistanceFromDessert = 4f;

    [Header("Nest Distance")]
    [SerializeField] private float minDistanceBetweenNests = 3f;

    [Header("Wave Settings")]
    public int numberOfNests = 2;
    public int linesPerNest = 1;

    [Header("Ants")]
    public List<GameObject> antPrefabs = new List<GameObject>();

    private int maxUnlockedAnt = 0;

    private List<Vector2> placedNestPositions = new List<Vector2>();

    private List<GameObject> spawnedNests = new List<GameObject>();
    private List<GameObject> spawnedLines = new List<GameObject>();

    private int pendingSpawnLines = 0;

    // =========================================================
    // EVENTS
    // =========================================================

    public static event System.Action OnAntSpawned;
    public static event System.Action OnSpawnComplete;

    // =========================================================
    // UNITY
    // =========================================================

    private void OnEnable()
    {
        GameManager.OnAddAntNest += AddNest;
        GameManager.OnMoreAntInLine += AddAntInLine;
        GameManager.OnNewAntType += UnlockNewAnt;
    }

    private void OnDisable()
    {
        GameManager.OnAddAntNest -= AddNest;
        GameManager.OnMoreAntInLine -= AddAntInLine;
        GameManager.OnNewAntType -= UnlockNewAnt;
    }

    private void Start()
    {
        numberOfNests = 2;
        linesPerNest = 1;
    }

    // =========================================================
    // WAVE
    // =========================================================

    public void StartWave()
    {
        ClearPreviousWave();

        pendingSpawnLines = 0;

        SpawnNests();

        // No lines means spawning is already complete.
        if (pendingSpawnLines == 0)
        {
            OnSpawnComplete?.Invoke();
        }
    }

    public void ClearPreviousWave()
    {
        StopAllCoroutines();

        foreach (GameObject nest in spawnedNests)
        {
            if (nest != null)
                Destroy(nest);
        }

        spawnedNests.Clear();

        foreach (GameObject line in spawnedLines)
        {
            if (line != null)
                Destroy(line);
        }

        spawnedLines.Clear();

        GameObject[] ants = GameObject.FindGameObjectsWithTag("Ant");

        foreach (GameObject ant in ants)
        {
            if (ant != null)
                Destroy(ant);
        }

        placedNestPositions.Clear();

        pendingSpawnLines = 0;
    }

    // =========================================================
    // NESTS
    // =========================================================

    private void SpawnNests()
    {
        for (int i = 0; i < numberOfNests; i++)
        {
            Vector2 nestPosition = GetNestPosition(i);

            placedNestPositions.Add(nestPosition);

            GameObject nest = Instantiate(
                antNest,
                nestPosition,
                Quaternion.identity
            );

            spawnedNests.Add(nest);

            SpawnLinePerNest(nest.transform);
        }
    }

    private Vector2 GetNestPosition(int index)
    {
        const int maxAttempts = 50;

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            Vector2 candidate = new Vector2(
                Random.Range(xMin, xMax),
                Random.Range(yMin, yMax)
            );

            // Don't spawn nest too close to dessert.
            if (dessertTransform != null)
            {
                float distanceFromDessert =
                    Vector2.Distance(candidate, dessertTransform.position);

                if (distanceFromDessert < minDistanceFromDessert)
                    continue;
            }

            // Don't overlap another nest.
            bool tooCloseToAnotherNest = false;

            foreach (Vector2 existingPosition in placedNestPositions)
            {
                if (Vector2.Distance(candidate, existingPosition)
                    < minDistanceBetweenNests)
                {
                    tooCloseToAnotherNest = true;
                    break;
                }
            }

            if (tooCloseToAnotherNest)
                continue;

            return candidate;
        }

        // Fallback if no valid position was found.
        Debug.LogWarning(
            "[SpawnManager] Couldn't find a perfect nest position. Using random position."
        );

        return new Vector2(
            Random.Range(xMin, xMax),
            Random.Range(yMin, yMax)
        );
    }

    // =========================================================
    // LINES
    // =========================================================

    private void SpawnLinePerNest(Transform nestTransform)
    {
        for (int i = 0; i < linesPerNest; i++)
        {
            GameObject line = new GameObject(
                nestTransform.name + "_Line_" + i
            );

            AntLineController lineController =
                line.AddComponent<AntLineController>();

            spawnedLines.Add(line);

            GameObject lineOrigin = new GameObject(
                nestTransform.name + "_LineOrigin_" + i
            );

            spawnedLines.Add(lineOrigin);

            Vector2 lineOffset =
                Random.insideUnitCircle.normalized * 0.9f * i;

            lineOrigin.transform.position =
                (Vector2)nestTransform.position + lineOffset;

            lineController.nest = lineOrigin.transform;

            pendingSpawnLines++;

            StartCoroutine(SpawnLine(lineController));
        }
    }

    private IEnumerator SpawnLine(AntLineController lineController)
    {
        for (int i = 0; i < lineController.maxAnts; i++)
        {
            SpawnAnt(lineController);

            yield return new WaitForSeconds(spawnDelay);
        }

        pendingSpawnLines--;

        if (pendingSpawnLines <= 0)
        {
            pendingSpawnLines = 0;
            OnSpawnComplete?.Invoke();
        }
    }

    // =========================================================
    // ANT
    // =========================================================

    private void SpawnAnt(AntLineController lineController)
    {
        if (antPrefabs == null || antPrefabs.Count == 0)
        {
            Debug.LogError(
                "[SpawnManager] No ant prefabs assigned!"
            );

            return;
        }

        int maxIndex = Mathf.Min(
            maxUnlockedAnt,
            antPrefabs.Count - 1
        );

        int index = Random.Range(0, maxIndex + 1);

        GameObject selectedAntPrefab = antPrefabs[index];

        if (selectedAntPrefab == null)
        {
            Debug.LogError(
                $"[SpawnManager] Ant prefab at index {index} is null!"
            );

            return;
        }

        Vector2 spawnPosition = lineController.nest.position;

        GameObject ant = Instantiate(
            selectedAntPrefab,
            spawnPosition,
            Quaternion.identity
        );

        if (ant == null)
            return;

        AntMovement antMovement =
            ant.GetComponent<AntMovement>();

        if (antMovement == null)
        {
            Debug.LogError(
                $"[SpawnManager] {selectedAntPrefab.name} doesn't have AntMovement!"
            );

            return;
        }

        antMovement.antLineController = lineController;

        lineController.antLine.Add(ant);
        lineController.UpdatePosition();

        // Tell WaveManager that one ant exists.
        OnAntSpawned?.Invoke();
    }

    // =========================================================
    // DIFFICULTY
    // =========================================================

    private void UnlockNewAnt()
    {
        if (antPrefabs == null || antPrefabs.Count == 0)
            return;

        if (maxUnlockedAnt < antPrefabs.Count - 1)
        {
            maxUnlockedAnt++;
        }
    }

    private void AddNest()
    {
        numberOfNests++;
    }

    private void AddAntInLine()
    {
        linesPerNest++;
    }

    // =========================================================
    // MAP EXPANSION
    // =========================================================

    public void ExpandSpawnArea(float amount)
    {
        xMin -= amount;
        xMax += amount;

        yMin -= amount;
        yMax += amount;

        Debug.Log(
            $"[SpawnManager] Spawn area expanded by {amount}."
        );
    }
    public void SetSpawnArea(
    float newXMin,
    float newXMax,
    float newYMin,
    float newYMax)
    {
        xMin = newXMin;
        xMax = newXMax;
        yMin = newYMin;
        yMax = newYMax;

        Debug.Log(
            $"[SpawnManager] Spawn area updated: " +
            $"X({xMin} → {xMax}), Y({yMin} → {yMax})"
        );
    }
}