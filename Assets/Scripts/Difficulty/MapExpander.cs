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
    [SerializeField] private float backgroundPadding = 2f;
    [SerializeField] private float spawnMargin = 1f;
    [SerializeField] private float maxCameraSize = 7f;
    private void OnEnable()
    {
        WaveReadySignal.OnWaveReady += HandlePlanningStarted;
    }

    private void OnDisable()
    {
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

    private void ExpandMap()
    {
        ExpandCamera();
        UpdateBackground();
        UpdateSpawnArea();

        Debug.Log(
            $"[MapExpander] Map expanded before Wave {waveManager.CurrentWave + 1}."
        );
    }

    private void UpdateSpawnArea()
    {
        if (spawnManager == null || mainCamera == null)
            return;

        float height = mainCamera.orthographicSize * 2f;
        float width = height * mainCamera.aspect;

        float xMin = mainCamera.transform.position.x - width / 2f + spawnMargin;
        float xMax = mainCamera.transform.position.x + width / 2f - spawnMargin;

        float yMin = mainCamera.transform.position.y - height / 2f + spawnMargin;
        float yMax = mainCamera.transform.position.y + height / 2f - spawnMargin;

        spawnManager.SetSpawnArea(
            xMin,
            xMax,
            yMin,
            yMax
        );
    }

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

}