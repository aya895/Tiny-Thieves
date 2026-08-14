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
    [SerializeField] private float spawnMargin = 1f;

    [Header("Camera Limit")]
    [SerializeField] private float maxCameraSize = 7f;


    // =========================================================
    // UNITY
    // =========================================================

    private void OnEnable()
    {
        WaveReadySignal.OnWaveReady += HandlePlanningStarted;
    }

    private void OnDisable()
    {
        WaveReadySignal.OnWaveReady -= HandlePlanningStarted;
    }


    // =========================================================
    // WAVE / EXPANSION
    // =========================================================

    private void HandlePlanningStarted()
    {
        if (waveManager == null)
            return;

        int nextWave = waveManager.CurrentWave + 1;

        // Expand every X waves.
        if (nextWave <= 0 ||
            nextWave % mapExpansionInterval != 0)
        {
            return;
        }

        ExpandMap();
    }


    private void ExpandMap()
    {
        ExpandCamera();

        UpdateBackground();

        UpdateSpawnArea();

        Debug.Log(
            $"[MapExpander] Map expanded before Wave " +
            $"{waveManager.CurrentWave + 1}."
        );
    }


    // =========================================================
    // CAMERA
    // =========================================================

    private void ExpandCamera()
    {
        if (mainCamera == null)
            return;

        if (!mainCamera.orthographic)
            return;

        float targetSize =
            mainCamera.orthographicSize + expansionAmount;

        mainCamera.orthographicSize =
            Mathf.Min(targetSize, maxCameraSize);

        Debug.Log(
            $"[MapExpander] Camera size: " +
            $"{mainCamera.orthographicSize}"
        );
    }


    // =========================================================
    // BACKGROUND
    // =========================================================

    private void UpdateBackground()
    {
        if (mainCamera == null || background == null)
            return;

        if (!mainCamera.orthographic)
            return;

        float height =
            mainCamera.orthographicSize * 2f;

        float width =
            height * mainCamera.aspect;

        // Resize background to cover the camera.
        background.size = new Vector2(
            width + backgroundPadding,
            height + backgroundPadding
        );

        // Keep background centered with camera.
        Vector3 backgroundPosition =
            background.transform.position;

        backgroundPosition.x =
            mainCamera.transform.position.x;

        backgroundPosition.y =
            mainCamera.transform.position.y;

        // Keep original Z.
        background.transform.position =
            backgroundPosition;
    }


    // =========================================================
    // SPAWN AREA
    // =========================================================

    private void UpdateSpawnArea()
    {
        if (spawnManager == null ||
            mainCamera == null)
        {
            return;
        }

        if (!mainCamera.orthographic)
            return;

        float height =
            mainCamera.orthographicSize * 2f;

        float width =
            height * mainCamera.aspect;


        float xMin =
            mainCamera.transform.position.x
            - width / 2f
            + spawnMargin;

        float xMax =
            mainCamera.transform.position.x
            + width / 2f
            - spawnMargin;


        float yMin =
            mainCamera.transform.position.y
            - height / 2f
            + spawnMargin;

        float yMax =
            mainCamera.transform.position.y
            + height / 2f
            - spawnMargin;


        spawnManager.SetSpawnArea(
            xMin,
            xMax,
            yMin,
            yMax
        );


        Debug.Log(
            $"[MapExpander] Spawn Area Updated: " +
            $"X({xMin} → {xMax}), " +
            $"Y({yMin} → {yMax})"
        );
    }
}