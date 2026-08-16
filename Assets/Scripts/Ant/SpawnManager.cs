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

    [Header("Dessert Distance")]
    [SerializeField] private Transform dessertTransform;
    [SerializeField] private float minDistanceFromDessert = 4f;

    [Header("Nest Distance")]
    [SerializeField] private float minDistanceBetweenNests = 3f;

    [Header("Wave Settings")]
    public int numberOfNests = 2;
    public int linesPerNest = 1;

    [Header("Line Settings")]
    [SerializeField] private float lineSpacing = 0.5f;

    [Header("Ants")]
    public List<GameObject> antPrefabs =
        new List<GameObject>();

    private int maxUnlockedAnt = 0;

    private readonly List<Vector2> placedNestPositions =
        new List<Vector2>();

    private readonly List<GameObject> spawnedNests =
        new List<GameObject>();

    private readonly List<GameObject> spawnedLines =
        new List<GameObject>();

    private int pendingSpawnLines = 0;


    // =========================================================
    // EVENTS
    // =========================================================

    public static event System.Action OnAntSpawned;
    public static event System.Action OnSpawnComplete;


    // =========================================================
    // UNITY
    // =========================================================

    private void Awake()
    {
        positionCalculator =
            new NestPositionCalculate(
                xMin,
                xMax,
                yMin,
                yMax,
                minDistanceBetweenNests,
                minDistanceFromDessert,
                dessertTransform
            );
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


    // =========================================================
    // WAVE
    // =========================================================

    public void StartWave()
    {
        ClearPreviousWave();

        pendingSpawnLines = 0;

        SpawnNests();

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

        GameObject[] ants =
            GameObject.FindGameObjectsWithTag("Ant");

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


    // =========================================================
    // NEST
    // =========================================================

    private void SpawnNests()
    {
        for (int i = 0;
             i < numberOfNests;
             i++)
        {
            if (!positionCalculator.TryGetNestPosition(
                    placedNestPositions,
                    out Vector2 nestPosition))
            {
                Debug.LogWarning(
                    "Could not find a valid position for another nest."
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

    private void SpawnLinePerNest(
        Transform nestTransform)
    {
        for (int i = 0;
             i < linesPerNest;
             i++)
        {
            GameObject line =
                new GameObject(
                    nestTransform.name +
                    "_Line_" +
                    i
                );

            AntLineController lineController =
                line.AddComponent<AntLineController>();

            spawnedLines.Add(line);


            GameObject lineOrigin =
                new GameObject(
                    nestTransform.name +
                    "_LineOrigin_" +
                    i
                );

            spawnedLines.Add(lineOrigin);


            Vector2 linePosition =
                nestTransform.position;

            // Additional lines move toward the dessert,
            // instead of randomly moving outside the camera.
            if (i > 0 &&
                dessertTransform != null)
            {
                Vector2 directionToDessert =
                    (
                        (Vector2)dessertTransform.position -
                        (Vector2)nestTransform.position
                    ).normalized;

                linePosition +=
                    directionToDessert *
                    lineSpacing *
                    i;
            }

            // Final safety check:
            // line origin can never leave spawn bounds.
            linePosition.x =
                Mathf.Clamp(
                    linePosition.x,
                    xMin,
                    xMax
                );

            linePosition.y =
                Mathf.Clamp(
                    linePosition.y,
                    yMin,
                    yMax
                );

            lineOrigin.transform.position =
                linePosition;


            lineController.nest =
                lineOrigin.transform;

            pendingSpawnLines++;

            StartCoroutine(
                SpawnLine(lineController)
            );
        }
    }

    private IEnumerator SpawnLine(
        AntLineController lineController)
    {
        for (int i = 0;
             i < lineController.maxAnts;
             i++)
        {
            SpawnAnt(lineController);

            yield return new WaitForSeconds(
                spawnDelay
            );
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

    private void SpawnAnt(
        AntLineController lineController)
    {
        if (antPrefabs == null ||
            antPrefabs.Count == 0)
        {
            return;
        }

        int maxIndex =
            Mathf.Min(
                maxUnlockedAnt,
                antPrefabs.Count - 1
            );

        int index =
            Random.Range(
                0,
                maxIndex + 1
            );

        GameObject selectedAntPrefab =
            antPrefabs[index];

        if (selectedAntPrefab == null)
            return;


        Vector2 spawnPosition =
            lineController.nest.position;

        // Safety clamp.
        spawnPosition.x =
            Mathf.Clamp(
                spawnPosition.x,
                xMin,
                xMax
            );

        spawnPosition.y =
            Mathf.Clamp(
                spawnPosition.y,
                yMin,
                yMax
            );


        GameObject ant =
            Instantiate(
                selectedAntPrefab,
                spawnPosition,
                Quaternion.identity
            );

        if (ant == null)
            return;


        AntMovement antMovement =
            ant.GetComponent<AntMovement>();

        if (antMovement != null)
        {
            antMovement.antLineController =
                lineController;

            lineController.antLine.Add(ant);

            lineController.UpdatePosition();
        }

        OnAntSpawned?.Invoke();
    }


    // =========================================================
    // DIFFICULTY
    // =========================================================

    private void UnlockNewAnt()
    {
        if (antPrefabs != null &&
            maxUnlockedAnt <
            antPrefabs.Count - 1)
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
    // SPAWN AREA
    // =========================================================

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

        positionCalculator.UpdateArea(
            xMin,
            xMax,
            yMin,
            yMax
        );
    }
}