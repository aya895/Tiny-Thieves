using System;
using System.Collections;
using UnityEngine;

// SINGLE RESPONSIBILITY: this class only knows about fuse timing, explosion
// radius, and chain propagation to the next TNT in line. It does not touch
// Animator, ParticleSystem, AudioSource, or Ant - see TNTVisual.cs,
// ExplosionSignal.cs, and ShockwaveSignal.cs for those.
public class TNTLogic : MonoBehaviour
{
    [Header("Explosion")]
    [SerializeField] private float explosionRadius = 2f;
    [SerializeField] private float damage = 100f;

    [Header("Shockwave / Knockback")]
    [Tooltip("How much bigger the physics push radius is than the damage radius.")]
    [SerializeField] private float shockwaveRadiusMultiplier = 1.4f;
    [Tooltip("Force applied to anything with a Rigidbody2D inside the shockwave radius.")]
    [SerializeField] private float knockbackForce = 8f;

    [Header("Fuse")]
    [Tooltip("How fast the spark travels down the fuse, in units/second. " +
             "Distance to the next TNT divided by this = the chain delay.")]
    [SerializeField] private float fuseBurnSpeed = 5f;

    public float ExplosionRadius =>
        explosionRadius + (PlayerUpgradeStats.Instance != null ? PlayerUpgradeStats.Instance.BonusExplosionRadius : 0f);

    // Single source of truth for the push radius - TNTVisual reads this
    // instead of keeping its own separate multiplier, so the visual ring
    // and the actual force radius can never drift apart.
    public float ShockwaveRadius => explosionRadius * shockwaveRadiusMultiplier;

    private float EffectiveKnockbackForce =>
        knockbackForce + (PlayerUpgradeStats.Instance != null ? PlayerUpgradeStats.Instance.BonusKnockbackForce : 0f);
        
     private float EffectiveFuseBurnSpeed =>
        fuseBurnSpeed + (PlayerUpgradeStats.Instance != null ? PlayerUpgradeStats.Instance.BonusFuseBurnSpeed : 0f);

    // Set by TNTPlacementController when this TNT is linked to the next one
    // placed in the chain.
    private TNTLogic nextInChain;
    private float distanceToNext;

    private bool hasIgnited = false;

    // Other systems (visuals, audio, score) subscribe to these instead of
    // TNTLogic calling them directly. Dependency Inversion: TNTLogic depends
    // on nothing; everyone else depends on TNTLogic's public events.
    public event Action OnExplode;

    public void SetNext(TNTLogic next, float distance)
    {
        nextInChain = next;
        distanceToNext = distance;
    }

    // Call this on the FIRST TNT in the chain from your Detonator.
    // Every subsequent TNT ignites itself once the fuse "reaches" it.
    public void Ignite()
    {
        if (hasIgnited) return;
        hasIgnited = true;


        StartCoroutine(ExplodeThenPropagate());
    }

    private IEnumerator ExplodeThenPropagate()
    {
        // Small pause so the spark visibly reaches the TNT body before it pops -
        // tune or remove this depending on how your fuse animation is timed.
        yield return new WaitForSeconds(0.15f);

        OnExplode?.Invoke();
        
        ExplosionSignal.Raise(transform.position, ExplosionRadius, damage);
        ShockwaveSignal.Raise(transform.position, ShockwaveRadius, EffectiveKnockbackForce);

        if (nextInChain != null)
        {
            float chainDelay = distanceToNext / EffectiveFuseBurnSpeed;
            yield return new WaitForSeconds(chainDelay);
            nextInChain.Ignite();
        }

        Destroy(gameObject, 0.1f);
    }

    // EDITOR-ONLY indicator - draws both radii as wire circles in the Scene
    // view, no Play mode required, no prefab wiring needed. Great for
    // eyeballing whether explosionRadius and shockwaveRadiusMultiplier feel
    // right before ever pressing Play.
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, ExplosionRadius);

        Gizmos.color = new Color(1f, 0.6f, 0.1f);
        Gizmos.DrawWireSphere(transform.position, EffectiveKnockbackForce);
    }
}