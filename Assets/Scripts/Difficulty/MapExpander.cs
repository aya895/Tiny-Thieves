using UnityEngine;

public class MapExpander : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpawnManager spawnManager;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private SpriteRenderer background;

    [Header("Expansion Settings")]
    [SerializeField] private float expansionAmount = 0.5f;
<<<<<<< Updated upstream
    [SerializeField] private float backgroundPadding = 2f;
    [SerializeField] private float spawnMargin = 1f;
    [SerializeField] private float maxCameraSize = 7f;
    private void OnEnable()
    {
        WaveReadySignal.OnWaveReady += HandlePlanningStarted;
=======
    [SerializeField] private float maxCameraSize = 7f;

    [Header("Background")]
    [SerializeField] private float backgroundPadding = 2f;

    [Header("Spawn Area")]
    [SerializeField] private float spawnMargin = 1.5f;


    private void OnEnable()
    {
        GameManager.OnAddAntNest += ExpandMap;
>>>>>>> Stashed changes
    }

    private void OnDisable()
    {
<<<<<<< Updated upstream
        WaveReadySignal.OnWaveReady -= HandlePlanningStarted;
    }

    private void HandlePlanningStarted()
    {
        if (waveManager == null)
            return;

        int nextWave = waveManager.CurrentWave + 1;

        if (nextWave <= 0 || nextWave % mapExpansionInterval != 0)
            return;

        ExpandMap();
    }

=======
        GameManager.OnAddAntNest -= ExpandMap;
    }

    private void Start()
    {
        UpdateBackground();
        UpdateSpawnArea();
    }


    // =========================================================
    // MAP EXPANSION
    // =========================================================

>>>>>>> Stashed changes
    private void ExpandMap()
    {
        if (!CanExpand())
            return;

        ExpandCamera();
        UpdateBackground();
        UpdateSpawnArea();

<<<<<<< Updated upstream
        Debug.Log(
            $"[MapExpander] Map expanded before Wave {waveManager.CurrentWave + 1}."
        );
=======
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
        mainCamera.orthographicSize = Mathf.Min(
            mainCamera.orthographicSize + expansionAmount,
            maxCameraSize
        );
    }


    // =========================================================
    // BACKGROUND
    // =========================================================

    private void UpdateBackground()
    {
        if (background == null || mainCamera == null)
            return;

        Vector2 cameraSize = GetCameraWorldSize();

        background.size =
            cameraSize + Vector2.one * backgroundPadding;

        Vector3 position = background.transform.position;

        position.x = mainCamera.transform.position.x;
        position.y = mainCamera.transform.position.y;

        background.transform.position = position;
>>>>>>> Stashed changes
    }

    private void UpdateSpawnArea()
    {
        if (spawnManager == null || mainCamera == null)
            return;

<<<<<<< Updated upstream
        float height = mainCamera.orthographicSize * 2f;
        float width = height * mainCamera.aspect;

        float xMin = mainCamera.transform.position.x - width / 2f + spawnMargin;
        float xMax = mainCamera.transform.position.x + width / 2f - spawnMargin;

        float yMin = mainCamera.transform.position.y - height / 2f + spawnMargin;
        float yMax = mainCamera.transform.position.y + height / 2f - spawnMargin;
=======
        Vector2 cameraSize = GetCameraWorldSize();
        Vector3 cameraPosition = mainCamera.transform.position;

        float halfWidth = cameraSize.x * 0.5f;
        float halfHeight = cameraSize.y * 0.5f;

        float xMin =
            cameraPosition.x - halfWidth + spawnMargin;

        float xMax =
            cameraPosition.x + halfWidth - spawnMargin;

        float yMin =
            cameraPosition.y - halfHeight + spawnMargin;

        float yMax =
            cameraPosition.y + halfHeight - spawnMargin;
>>>>>>> Stashed changes

        spawnManager.SetSpawnArea(
            xMin,
            xMax,
            yMin,
            yMax
        );
    }

<<<<<<< Updated upstream
    private void ExpandCamera()
    {
        if (mainCamera == null || !mainCamera.orthographic)
            return;

        float targetSize = mainCamera.orthographicSize + expansionAmount;

        mainCamera.orthographicSize = Mathf.Min(
            targetSize,
            maxCameraSize
        );
    }

    //private void LateUpdate()
    //{
    //    UpdateBackground();
    //}

    private void UpdateBackground()
    {
        if (mainCamera == null || background == null)
            return;

        float height = mainCamera.orthographicSize * 2f;
        float width = height * mainCamera.aspect;

        background.size = new Vector2(
            width + backgroundPadding,
            height + backgroundPadding
        );

        // Move only on X and Y.
        background.transform.position = new Vector3(
            mainCamera.transform.position.x,
            mainCamera.transform.position.y,
            background.transform.position.z
        );
    }

=======

    // =========================================================
    // HELPERS
    // =========================================================

    private Vector2 GetCameraWorldSize()
    {
        float height = mainCamera.orthographicSize * 2f;
        float width = height * mainCamera.aspect;

        return new Vector2(width, height);
    }
>>>>>>> Stashed changes
}