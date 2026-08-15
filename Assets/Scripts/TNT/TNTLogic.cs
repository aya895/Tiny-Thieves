using System;
using System.Collections;
using UnityEngine;

public class TNTLogic : MonoBehaviour
{
    [Header("Explosion")]
    [SerializeField] private float explosionRadius = 2f;
    [SerializeField] private float damage = 100f;

    [Header("Shockwave")]
    [SerializeField] private float shockwaveRadiusMultiplier = 1.4f;
    [SerializeField] private float knockbackForce = 8f;

    [Header("Fuse")]
    [SerializeField] private float fuseBurnSpeed = 5f;

    private PlayerUpgradeStats playerStats;
    private TNTPlacementController placementController;

    private TNTLogic nextInChain;
    private float distanceToNext;

    private bool hasIgnited;

    // TNTVisual and FuseConnection listen to this.
    public event Action OnExplode;

    public float BaseExplosionRadius =>
        explosionRadius;

    public float ExplosionRadius =>
        explosionRadius +
        (playerStats != null
            ? playerStats.BonusExplosionRadius
            : 0f);

    public float ShockwaveRadius =>
        ExplosionRadius *
        shockwaveRadiusMultiplier;

    private float EffectiveKnockbackForce =>
        knockbackForce +
        (playerStats != null
            ? playerStats.BonusKnockbackForce
            : 0f);

    private float EffectiveFuseBurnSpeed =>
        fuseBurnSpeed +
        (playerStats != null
            ? playerStats.BonusFuseBurnSpeed
            : 0f);

    public void Initialize(
        PlayerUpgradeStats stats,
        TNTPlacementController controller)
    {
        playerStats = stats;
        placementController = controller;
    }

    public void SetNext(
        TNTLogic next,
        float distance)
    {
        nextInChain = next;
        distanceToNext = distance;
    }

    public void Ignite()
    {
        if (hasIgnited)
            return;

        hasIgnited = true;

        StartCoroutine(
            ExplodeThenPropagate()
        );
    }

    private IEnumerator ExplodeThenPropagate()
    {
        yield return new WaitForSeconds(0.15f);

        // Visual / fuse event.
        OnExplode?.Invoke();

        // Gameplay explosion.
        if (placementController != null)
        {
            placementController.RaiseExplosion(
                transform.position,
                ExplosionRadius,
                damage
            );

            placementController.RaiseShockwave(
                transform.position,
                ShockwaveRadius,
                EffectiveKnockbackForce
            );
        }

        if (nextInChain != null)
        {
            float safeFuseSpeed =
                Mathf.Max(
                    EffectiveFuseBurnSpeed,
                    0.01f
                );

            float chainDelay =
                distanceToNext /
                safeFuseSpeed;

            yield return new WaitForSeconds(
                chainDelay
            );

            nextInChain.Ignite();
        }

        Destroy(gameObject, 0.1f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            transform.position,
            explosionRadius
        );

        Gizmos.color =
            new Color(
                1f,
                0.6f,
                0.1f
            );

        Gizmos.DrawWireSphere(
            transform.position,
            explosionRadius *
            shockwaveRadiusMultiplier
        );
    }
}