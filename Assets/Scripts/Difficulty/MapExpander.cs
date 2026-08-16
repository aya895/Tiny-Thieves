using UnityEngine;

public class MapExpander : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpawnManager spawnManager;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private SpriteRenderer background;
    [SerializeField] private WaveManager waveManager;

    [Header("Expansion Settings")]
    [SerializeField] private int mapExpansionInterval = 3;
    [SerializeField] private float expansionAmount = 0.5f;

    [Header("Background")]
    [SerializeField] private float backgroundPadding = 2f;

    [Header("Spawn Area")]
    [SerializeField] private float spawnMargin = 1.5f;

    [Header("Camera Limit")]
    [SerializeField] private float maxCameraSize = 7f;


    // =========================================================
    // UNITY
    // =========================================================

    private void OnEnable()
    {
        WaveManager.OnWaveReady += HandlePlanningStarted;
    }

    private void OnDisable()
    {
        WaveManager.OnWaveReady -= HandlePlanningStarted;
    }

    private void Start()
    {
        // IMPORTANT:
        // Set correct spawn bounds from the first wave.
        UpdateBackground();
        UpdateSpawnArea();
    }


    // =========================================================
    // WAVE / EXPANSION
    // =========================================================

    private void HandlePlanningStarted()
    {
        if (waveManager == null)
            return;

        int nextWave = waveManager.CurrentWave + 1;

        if (nextWave <= 0)
            return;

        if (nextWave % mapExpansionInterval != 0)
            return;

        ExpandMap();
    }

    private void ExpandMap()
    {
        if (!CanExpand())
            return;

        ExpandCamera();
        UpdateBackground();
        UpdateSpawnArea();
    }

    private bool CanExpand()
    {
        return mainCamera != null &&
               mainCamera.orthographic &&
               mainCamera.orthographicSize < maxCameraSize;
    }


    // =========================================================
    // CAMERA
    // =========================================================

    private void ExpandCamera()
    {
        mainCamera.orthographicSize =
            Mathf.Min(
                mainCamera.orthographicSize + expansionAmount,
                maxCameraSize
            );
    }


    // =========================================================
    // BACKGROUND
    // =========================================================

    private void UpdateBackground()
    {
        if (mainCamera == null ||
            background == null ||
            !mainCamera.orthographic)
        {
            return;
        }

        Vector2 cameraSize =
            GetCameraWorldSize();

        background.size =
            cameraSize +
            Vector2.one * backgroundPadding;

        Vector3 position =
            background.transform.position;

        position.x =
            mainCamera.transform.position.x;

        position.y =
            mainCamera.transform.position.y;

        background.transform.position =
            position;
    }


    // =========================================================
    // SPAWN AREA
    // =========================================================

    private void UpdateSpawnArea()
    {
        if (spawnManager == null ||
            mainCamera == null ||
            !mainCamera.orthographic)
        {
            return;
        }

        Vector2 cameraSize =
            GetCameraWorldSize();

        Vector3 cameraPosition =
            mainCamera.transform.position;

        float halfWidth =
            cameraSize.x * 0.5f;

        float halfHeight =
            cameraSize.y * 0.5f;

        float xMin =
            cameraPosition.x -
            halfWidth +
            spawnMargin;

        float xMax =
            cameraPosition.x +
            halfWidth -
            spawnMargin;

        float yMin =
            cameraPosition.y -
            halfHeight +
            spawnMargin;

        float yMax =
            cameraPosition.y +
            halfHeight -
            spawnMargin;

        spawnManager.SetSpawnArea(
            xMin,
            xMax,
            yMin,
            yMax
        );
    }


    // =========================================================
    // HELPERS
    // =========================================================

    private Vector2 GetCameraWorldSize()
    {
        float height =
            mainCamera.orthographicSize * 2f;

        float width =
            height * mainCamera.aspect;

        return new Vector2(
            width,
            height
        );
    }
}