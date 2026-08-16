using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

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
    private ObjectPool<GameObject> antPool;
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
        Ant.OnAntDeath += HandleAntDeath;
    }

    private void OnDisable()
    {
        GameManager.OnAddAntNest -= AddNest;
        GameManager.OnMoreAntInLine -= AddAntInLine;
        GameManager.OnNewAntType -= UnlockNewAnt;
        Ant.OnAntDeath -= HandleAntDeath;
    }

    private void Start()
    {
        numberOfNests = 2;
        linesPerNest = 1;

        antPool = new ObjectPool<GameObject>(CreateAnt, OnGetAnt, OnReleaseAnt, OnDestroyAnt, true, 50, 500);
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
            if (ant != null && ant.activeSelf)
            {
                ReleaseAnt(ant);
            }
        }
        placedNestPositions.Clear();
        pendingSpawnLines = 0;
    }

    private void SpawnNests()
    {
        for (int i = 0; i < numberOfNests; i++)
        {
            if (!positionCalculator.TryGetNestPosition(placedNestPositions,out Vector2 nestPosition))
            {
                break;
            }

            placedNestPositions.Add(nestPosition);
            GameObject nest =Instantiate(antNest,nestPosition,Quaternion.identity);
            spawnedNests.Add(nest);
            SpawnLinePerNest(nest.transform);
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

        GameObject ant = antPool.Get();
        if (ant == null) return;

        ant.transform.position = lineController.nest.position;
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

    public void SetSpawnArea(float newXMin, float newXMax, float newYMin, float newYMax)
    {
        xMin = newXMin;
        xMax = newXMax;
        yMin = newYMin;
        yMax = newYMax;
        positionCalculator.UpdateArea(xMin, xMax, yMin, yMax);
    }
}