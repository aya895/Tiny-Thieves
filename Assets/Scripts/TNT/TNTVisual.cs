using UnityEngine;

// SINGLE RESPONSIBILITY: this class ONLY plays animation/VFX/SFX. It has no
// idea what a "chain reaction" is - it just reacts to TNTLogic's events.
// This is the fix for review comment #1: swap or restyle the spark/explosion
// animation here without ever touching the gameplay logic in TNTLogic.
//
// Lives on the CHILD "visual" object. TNTLogic lives on the parent, so we
// look it up with GetComponentInParent instead of GetComponent/RequireComponent.
public class TNTVisual : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string sparkTriggerName = "Spark";
    [SerializeField] private string explodeTriggerName = "Explode";

    [SerializeField] private GameObject explosion;

    [Header("Blast Radius Flash")]
    [Tooltip("Shows a circle at the EXACT explosion radius, so players can see what was actually hit.")]
    [SerializeField] private ExplosionRadiusIndicator blastRadiusPrefab;
    [SerializeField] private float blastFlashDuration = 0.25f;

    [Header("Shockwave")]
    [Tooltip("A separate expanding ring, larger than the actual blast radius, purely for impact/juice.")]
    [SerializeField] private ShockwaveEffect shockwavePrefab;
    [SerializeField] private float shockwaveDuration = 0.4f;

    private int sparkHash;
    private int explodeHash;
    private TNTLogic logic;

    private void Awake()
    {
        logic = GetComponentInParent<TNTLogic>();
        sparkHash = Animator.StringToHash(sparkTriggerName);
        explodeHash = Animator.StringToHash(explodeTriggerName);
    }

    private void OnEnable()
    {
        logic.OnExplode += HandleExplode;
    }

    private void OnDisable()
    {
        logic.OnExplode -= HandleExplode;
    }

    private void HandleExplode(Vector2 position, float radius, float damage)
    {
        if (animator != null) animator.SetTrigger(explodeHash);
        explosion.SetActive(true);

        SpawnBlastRadiusFlash();
        SpawnShockwave();

        gameObject.SetActive(false);
    }

    // Circle at the exact explosion radius - "this is what got hit."
    private void SpawnBlastRadiusFlash()
    {
        if (blastRadiusPrefab == null) return;

        ExplosionRadiusIndicator flash = Instantiate(blastRadiusPrefab, logic.transform.position, Quaternion.identity);
        flash.SetRadius(logic.ExplosionRadius);
        flash.SetVisible(true);
        Destroy(flash.gameObject, blastFlashDuration);
    }

    // Bigger expanding ring, purely for visual punch - not tied to actual damage range.
    private void SpawnShockwave()
    {
        if (shockwavePrefab == null) return;

        ShockwaveEffect shockwave = Instantiate(shockwavePrefab, logic.transform.position, Quaternion.identity);
        shockwave.Play(logic.ExplosionRadius, logic.ShockwaveRadius, shockwaveDuration);
    }
}