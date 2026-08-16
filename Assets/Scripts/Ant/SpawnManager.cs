using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class SpawnManager : MonoBehaviour
{
    private NestPositionCalculate positionCalculator;


    // =========================================================
    // NEST
    // =========================================================

    [Header("Nest")]
    [SerializeField] private GameObject antNest;


    // =========================================================
    // SPAWN SETTINGS
    // =========================================================

    [Header("Spawn Settings")]
    [SerializeField] private float spawnDelay = 0.75f;

    [SerializeField] private float xMin = -8f;
    [SerializeField] private float xMax = -6f;
    [SerializeField] private float yMin = -4f;
    [SerializeField] private float yMax = 1f;


    // =========================================================
    // CAMERA BOUNDS
    // =========================================================

    [Header("Camera Bounds")]
    [SerializeField] private Camera gameCamera;

    [SerializeField] private float cameraPadding = 0.5f;


    // =========================================================
    // DESSERT DISTANCE
    // =========================================================

    [Header("Dessert Distance")]
    [SerializeField] private Transform dessertTransform;

    [SerializeField] private float minDistanceFromDessert = 4f;


    // =========================================================
    // NEST DISTANCE
    // =========================================================

    [Header("Nest Distance")]
    [SerializeField] private float minDistanceBetweenNests = 3f;


    // =========================================================
    // WAVE SETTINGS
    // =========================================================

    [Header("Wave Settings")]
    public int numberOfNests = 2;

    public int linesPerNest = 1;


    // =========================================================
    // ANTS
    // =========================================================

    [Header("Ants")]
    public List<GameObject> antPrefabs =
        new List<GameObject>();

    private ObjectPool<GameObject> antPool;
    private int maxUnlockedAnt = 0;


    // =========================================================
    // RUNTIME DATA
    // =========================================================

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
        if (gameCamera == null)
        {
            gameCamera = Camera.main;
        }

        UpdateSpawnAreaFromCamera();

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


    private void Start()
    {
        ResetProgress();
        antPool = new ObjectPool<GameObject>(CreateAnt, OnGetAnt, OnReleaseAnt, OnDestroyAnt, true, 50, 500);
    }

    private void OnEnable()
    {
        GameManager.OnAddAntNest += AddNest;
        GameManager.OnMoreAntInLine += AddAntInLine;
        GameManager.OnNewAntType += UnlockNewAnt;
        Ant.OnAntDeath += HandleAntDeath;
    }


    private void OnDisable()
    {
        GameManager.OnAddAntNest -= AddNest;
        GameManager.OnMoreAntInLine -= AddAntInLine;
        GameManager.OnNewAntType -= UnlockNewAnt;
        Ant.OnAntDeath -= HandleAntDeath;
    }


    // =========================================================
    // WAVE
    // =========================================================

    public void ResetProgress()
    {
        numberOfNests = 1;
        linesPerNest = 1;
        maxUnlockedAnt = 0;

        ClearPreviousWave();
    }

    public void StartWave()
    {
        ClearPreviousWave();
        UpdateSpawnAreaFromCamera();

        positionCalculator.UpdateArea(
            xMin,
            xMax,
            yMin,
            yMax
        );

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

        AntMovement[] activeAnts = FindObjectsByType<AntMovement>(FindObjectsSortMode.None);
        foreach (AntMovement antMovement in activeAnts)
        {
            if (antMovement != null && antMovement.gameObject.activeSelf)
            {
                ReleaseAnt(antMovement.gameObject);
            }
        }

        placedNestPositions.Clear();

        pendingSpawnLines = 0;
    }


    // =========================================================
    // NEST SPAWNING
    // =========================================================

    private void SpawnNests()
    {
        for (int i = 0; i < numberOfNests; i++)
        {
            if (!positionCalculator.TryGetNestPosition(placedNestPositions, out Vector2 nestPosition))
            {
                break;
            }

            nestPosition.x = Mathf.Clamp(nestPosition.x, xMin, xMax);
            nestPosition.y = Mathf.Clamp(nestPosition.y, yMin, yMax);

            placedNestPositions.Add(nestPosition);

            GameObject nest = Instantiate(antNest, nestPosition, Quaternion.identity);
            spawnedNests.Add(nest);

            SpawnLinePerNest(nest.transform);
        }
    }


    // =========================================================
    // LINES
    // =========================================================

    private void SpawnLinePerNest(
        Transform nestTransform)
    {
        for (int i = 0; i < linesPerNest; i++)
        {
            GameObject line = new GameObject($"{nestTransform.name}_Line_{i}");
            AntLineController lineController = line.AddComponent<AntLineController>();
            spawnedLines.Add(line);

            GameObject lineOrigin = new GameObject($"{nestTransform.name}_LineOrigin_{i}");
            spawnedLines.Add(lineOrigin);

            Vector2 lineOffset = Random.insideUnitCircle.normalized * 0.9f * i;
            Vector2 linePosition = (Vector2)nestTransform.position + lineOffset;

            // Clamp line origin to camera boundaries
            linePosition.x = Mathf.Clamp(linePosition.x, xMin, xMax);
            linePosition.y = Mathf.Clamp(linePosition.y, yMin, yMax);

            lineOrigin.transform.position = linePosition;
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
    // ANT SPAWNING
    // =========================================================

    private void SpawnAnt(AntLineController lineController)
    {
        if (antPrefabs == null || antPrefabs.Count == 0)
            return;

        Vector2 spawnPosition = lineController.nest.position;
        spawnPosition.x = Mathf.Clamp(spawnPosition.x, xMin, xMax);
        spawnPosition.y = Mathf.Clamp(spawnPosition.y, yMin, yMax);

        GameObject ant = antPool.Get();
        if (ant == null) return;

        ant.transform.position = spawnPosition;
        ant.transform.rotation = Quaternion.identity;

        AntMovement antMovement = ant.GetComponent<AntMovement>();
        if (antMovement != null)
        {
            antMovement.antLineController = lineController;
            lineController.antLine.Add(ant);
            lineController.UpdatePosition();
        }

        OnAntSpawned?.Invoke();
    }

    private void HandleAntDeath(GameObject antObject, float expValue)
    {
        ReleaseAnt(antObject);
    }

   // needed for object pooling
    private void ReleaseAnt(GameObject antObject)
    {
        if (antObject == null || !antObject.activeSelf)
            return;

        antPool.Release(antObject);
    }

    private GameObject CreateAnt()
    {
        int maxIndex = Mathf.Min(maxUnlockedAnt, antPrefabs.Count - 1);
        int index = Random.Range(0, maxIndex + 1);
        GameObject selectedPrefab = antPrefabs[index];

        return Instantiate(selectedPrefab);
    }

    private void OnGetAnt(GameObject instance) // lots of reset but its necessary :(
    {
        instance.transform.SetParent(null);
        AntStackController stacker = instance.GetComponent<AntStackController>();
        if (stacker != null)
        {
            stacker.DetachFromStack();
        }

        Ant ant = instance.GetComponent<Ant>();
        if (ant != null)
        {
            ant.ResetForPool();
        }

        AntMovement antMovement = instance.GetComponent<AntMovement>();
        if (antMovement != null)
        {
            antMovement.ResetMovement();
        }

        AntStats antStats = instance.GetComponent<AntStats>();
        if (antStats != null)
        {
            antStats.ResetStats();
        }

        Rigidbody2D rb = instance.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.simulated = true;
        }

        Collider2D col = instance.GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = true;
        }

        SpriteRenderer sr = instance.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sortingOrder = 0;
            sr.color = Color.white;
        }

        instance.SetActive(true);
    }

    private void OnReleaseAnt(GameObject instance)
    {
        instance.transform.SetParent(null);
        instance.SetActive(false);
    }

    private void OnDestroyAnt(GameObject instance)
    {
        Destroy(instance);
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
    // CAMERA / SPAWN AREA
    // =========================================================

    private void UpdateSpawnAreaFromCamera()
    {
        if (gameCamera == null)
            return;

        Vector3 bottomLeft = gameCamera.ViewportToWorldPoint(new Vector3(0f, 0f, 0f));
        Vector3 topRight = gameCamera.ViewportToWorldPoint(new Vector3(1f, 1f, 0f));

        xMin = bottomLeft.x + cameraPadding;
        xMax = topRight.x - cameraPadding;
        yMin = bottomLeft.y + cameraPadding;
        yMax = topRight.y - cameraPadding;
    }

    public void ExpandSpawnArea(float amount)
    {
        xMin -= amount;
        xMax += amount;
        yMin -= amount;
        yMax += amount;

        positionCalculator.UpdateArea(xMin, xMax, yMin, yMax);
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


        positionCalculator.UpdateArea(
            xMin,
            xMax,
            yMin,
            yMax
        );
    }
}