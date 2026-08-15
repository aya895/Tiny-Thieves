using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    private NestPositionCalculate positionCalculator;

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

    public static event System.Action OnAntSpawned;
    public static event System.Action OnSpawnComplete;

    private void Awake()
    {
        positionCalculator = new NestPositionCalculate(xMin, xMax, yMin, yMax,
            minDistanceBetweenNests, minDistanceFromDessert, dessertTransform);
    }

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
            {
                Destroy(nest);
            }
        }

        spawnedNests.Clear();

        foreach (GameObject line in spawnedLines)
        {
            if (line != null)
            {
                Destroy(line);
            }
        }

        spawnedLines.Clear();

        GameObject[] ants = GameObject.FindGameObjectsWithTag("Ant");

        foreach (GameObject ant in ants)
        {
            if (ant != null)
            {
                Destroy(ant);
            }
        }
        placedNestPositions.Clear();
        pendingSpawnLines = 0;
    }

    private void SpawnNests()
    {
        for (int i = 0; i < numberOfNests; i++)
        {
            if (!positionCalculator.TryGetNestPosition(
                    placedNestPositions,
                    out Vector2 nestPosition))
            {
                Debug.LogWarning(
                    "[SpawnManager] No valid nest position available."
                );

                break;
            }

            placedNestPositions.Add(
                nestPosition
            );

            GameObject nest =
                Instantiate(
                    antNest,
                    nestPosition,
                    Quaternion.identity
                );

            spawnedNests.Add(nest);

            SpawnLinePerNest(
                nest.transform
            );
        }
    }

    // =========================================================
    // LINES
    // =========================================================

    private void SpawnLinePerNest(Transform nestTransform)
    {
        for (int i = 0; i < linesPerNest; i++)
        {
            GameObject line = new GameObject(nestTransform.name + "_Line_" + i);
            AntLineController lineController = line.AddComponent<AntLineController>();
            spawnedLines.Add(line);

            GameObject lineOrigin = new GameObject(nestTransform.name + "_LineOrigin_" + i);
            spawnedLines.Add(lineOrigin);

            Vector2 lineOffset = Random.insideUnitCircle.normalized * 0.9f * i;
            lineOrigin.transform.position = (Vector2)nestTransform.position + lineOffset;

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
            return;
        }

        int maxIndex = Mathf.Min(maxUnlockedAnt,antPrefabs.Count - 1);
        int index = Random.Range(0, maxIndex + 1);

        GameObject selectedAntPrefab = antPrefabs[index];
        if (selectedAntPrefab == null)
        {
            return;
        }

        Vector2 spawnPosition = lineController.nest.position;

        GameObject ant = Instantiate(selectedAntPrefab,spawnPosition,Quaternion.identity);
        if (ant == null)
            return;

        AntMovement antMovement = ant.GetComponent<AntMovement>();
        if (antMovement != null)
        {
            antMovement.antLineController = lineController;
            lineController.antLine.Add(ant);
            lineController.UpdatePosition();
        }

        OnAntSpawned?.Invoke();
    }

    // =========================================================
    // DIFFICULTY & AREA EXPANSION
    // =========================================================

    private void UnlockNewAnt()
    {
        if (antPrefabs != null && maxUnlockedAnt < antPrefabs.Count - 1)
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

    public void ExpandSpawnArea(float amount)
    {
        xMin -= amount;
        xMax += amount;
        yMin -= amount;
        yMax += amount;
        positionCalculator.UpdateArea(xMin, xMax, yMin, yMax);
    }

    public void SetSpawnArea(float newXMin,float newXMax,float newYMin,float newYMax)
    {
        xMin = newXMin;
        xMax = newXMax;
        yMin = newYMin;
        yMax = newYMax;
        positionCalculator.UpdateArea(xMin, xMax, yMin, yMax);
    }
}