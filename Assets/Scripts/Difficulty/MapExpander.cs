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
        ExpandSpawnArea();
        ExpandCamera();
       //UpdateBackground();

        Debug.Log(
            $"[MapExpander] Map expanded before Wave {waveManager.CurrentWave + 1}."
        );
    }

    private void ExpandSpawnArea()
    {
        if (spawnManager == null)
            return;

        spawnManager.ExpandSpawnArea(expansionAmount);
    }

    private void ExpandCamera()
    {
        if (mainCamera == null || !mainCamera.orthographic)
            return;

        mainCamera.orthographicSize += expansionAmount;
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

        background.transform.position = mainCamera.transform.position;
    }

}