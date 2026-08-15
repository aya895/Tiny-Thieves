using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TNTPlacementController : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private TNTLogic tntPrefab;
    [SerializeField] private ExplosionRadiusIndicator previewIndicatorPrefab;
    [SerializeField] private FuseConnection fuseLinePrefab;

    [Header("Dependencies")]
    [SerializeField] private PlayerUpgradeStats playerUpgradeStats;
    [SerializeField] private WaveManager waveManager;

    [Header("Rules")]
    [SerializeField] private float maxFuseLength = 6f;
    [SerializeField] private int maxTNTCount = 5;

    // =========================================================
    // EVENTS
    // =========================================================

    public event Action<Vector2, float, float> OnAnyExplosion;
    public event Action<Vector2, float, float> OnAnyShockwave;

    // =========================================================
    // STATE
    // =========================================================

    private ExplosionRadiusIndicator activePreview;

    private TNTLogic firstPlaced;
    private TNTLogic lastPlaced;

    private Camera mainCam;
    private int placedCount;

    private readonly List<TNTLogic> placedTNTs =
        new List<TNTLogic>();

    private readonly List<FuseConnection> placedFuses =
        new List<FuseConnection>();

    // =========================================================
    // EFFECTIVE VALUES
    // =========================================================

    public int RemainingTNT =>
        Mathf.Max(
            0,
            EffectiveMaxTNTCount - placedCount
        );

    private int EffectiveMaxTNTCount =>
        maxTNTCount +
        (playerUpgradeStats != null
            ? playerUpgradeStats.BonusMaxTNTCount
            : 0);

    private float EffectiveMaxFuseLength =>
        maxFuseLength +
        (playerUpgradeStats != null
            ? playerUpgradeStats.BonusMaxFuseDistance
            : 0f);

    private float EffectiveExplosionRadius =>
        tntPrefab.BaseExplosionRadius +
        (playerUpgradeStats != null
            ? playerUpgradeStats.BonusExplosionRadius
            : 0f);

    // =========================================================
    // UNITY
    // =========================================================

    private void Awake()
    {
        mainCam = Camera.main;
    }

    private void OnEnable()
    {
        WaveManager.OnWaveReady += ResetForNewWave;
        WaveManager.OnWaveEnded += ClearPlacedObjects;
    }

    private void OnDisable()
    {
        WaveManager.OnWaveReady -= ResetForNewWave;
        WaveManager.OnWaveEnded -= ClearPlacedObjects;
    }

    private void Update()
    {
        if (waveManager == null)
            return;

        if (!waveManager.IsPlanning() &&
            !waveManager.IsPlaying())
        {
            HidePreview();
            return;
        }

        if (mainCam == null)
            return;

        Vector3 mouseWorld =
            mainCam.ScreenToWorldPoint(
                Input.mousePosition
            );

        mouseWorld.z = 0f;

        bool placementValid =
            IsPlacementValid(mouseWorld);

        UpdatePreview(
            mouseWorld,
            placementValid
        );

        if (!Input.GetMouseButtonDown(0))
            return;

        // Do not place TNT when clicking UI.
        if (EventSystem.current != null &&
            EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (placementValid)
        {
            PlaceTNT(mouseWorld);
        }
    }

    // =========================================================
    // EXPLOSION BROADCASTING
    // =========================================================

    public void RaiseExplosion(
        Vector2 position,
        float radius,
        float damage)
    {
        OnAnyExplosion?.Invoke(
            position,
            radius,
            damage
        );
    }

    public void RaiseShockwave(
        Vector2 position,
        float radius,
        float force)
    {
        OnAnyShockwave?.Invoke(
            position,
            radius,
            force
        );
    }

    // =========================================================
    // WAVE CLEANUP
    // =========================================================

    private void ClearPlacedObjects()
    {
        foreach (TNTLogic tnt in placedTNTs)
        {
            if (tnt != null)
            {
                Destroy(tnt.gameObject);
            }
        }

        placedTNTs.Clear();

        foreach (FuseConnection fuse in placedFuses)
        {
            if (fuse != null)
            {
                Destroy(fuse.gameObject);
            }
        }

        placedFuses.Clear();

        placedCount = 0;
        firstPlaced = null;
        lastPlaced = null;

        DestroyPreview();
    }

    private void ResetForNewWave()
    {
        ClearPlacedObjects();
    }

    // =========================================================
    // PREVIEW
    // =========================================================

    private void UpdatePreview(
        Vector3 worldPosition,
        bool placementValid)
    {
        if (placedCount >= EffectiveMaxTNTCount)
        {
            HidePreview();
            return;
        }

        if (activePreview == null)
        {
            activePreview =
                Instantiate(
                    previewIndicatorPrefab
                );
        }

        activePreview.transform.position =
            worldPosition;

        activePreview.SetRadius(
            EffectiveExplosionRadius
        );

        activePreview.SetVisible(
            placementValid
        );
    }

    private void HidePreview()
    {
        if (activePreview != null)
        {
            activePreview.SetVisible(false);
        }
    }

    private void DestroyPreview()
    {
        if (activePreview == null)
            return;

        Destroy(activePreview.gameObject);

        activePreview = null;
    }

    // =========================================================
    // PLACEMENT
    // =========================================================

    private bool IsPlacementValid(
        Vector3 worldPosition)
    {
        if (placedCount >= EffectiveMaxTNTCount)
            return false;

        if (lastPlaced == null)
            return true;

        float allowedDistance =
            EffectiveMaxFuseLength;

        Vector3 difference =
            worldPosition -
            lastPlaced.transform.position;

        return difference.sqrMagnitude <=
               allowedDistance * allowedDistance;
    }

    private void PlaceTNT(
        Vector3 worldPosition)
    {
        TNTLogic newTNT =
            Instantiate(
                tntPrefab,
                worldPosition,
                Quaternion.identity
            );

        newTNT.Initialize(
            playerUpgradeStats,
            this
        );

        placedTNTs.Add(newTNT);

        placedCount++;

        if (lastPlaced != null)
        {
            FuseConnection fuse =
                Instantiate(
                    fuseLinePrefab
                );

            placedFuses.Add(fuse);

            float distance =
                fuse.Setup(
                    lastPlaced,
                    worldPosition
                );

            lastPlaced.SetNext(
                newTNT,
                distance
            );
        }
        else
        {
            firstPlaced = newTNT;
        }

        lastPlaced = newTNT;
    }

    public TNTLogic GetChainStart()
    {
        return firstPlaced;
    }
}