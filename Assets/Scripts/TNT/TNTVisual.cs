using UnityEngine;

public class TNTVisual : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string sparkTriggerName = "Spark";
    [SerializeField] private string explodeTriggerName = "Explode";

    [SerializeField] private GameObject explosion;

    [Header("Blast Radius Flash")]
    [SerializeField]
    private ExplosionRadiusIndicator blastRadiusPrefab;

    [SerializeField]
    private float blastFlashDuration = 0.25f;

    [Header("Shockwave")]
    [SerializeField]
    private ShockwaveEffect shockwavePrefab;

    [SerializeField]
    private float shockwaveDuration = 0.4f;

    private int sparkHash;
    private int explodeHash;

    private TNTLogic logic;

    private void Awake()
    {
        logic =
            GetComponentInParent<TNTLogic>();

        sparkHash =
            Animator.StringToHash(
                sparkTriggerName
            );

        explodeHash =
            Animator.StringToHash(
                explodeTriggerName
            );
    }

    private void OnEnable()
    {
        if (logic != null)
        {
            logic.OnExplode +=
                HandleExplode;
        }
    }

    private void OnDisable()
    {
        if (logic != null)
        {
            logic.OnExplode -=
                HandleExplode;
        }
    }

    private void HandleExplode()
    {
        if (animator != null)
        {
            animator.SetTrigger(
                explodeHash
            );
        }

        if (explosion != null)
        {
            explosion.SetActive(true);
        }

        SpawnBlastRadiusFlash();
        SpawnShockwave();

        gameObject.SetActive(false);
    }

    private void SpawnBlastRadiusFlash()
    {
        if (blastRadiusPrefab == null ||
            logic == null)
        {
            return;
        }

        ExplosionRadiusIndicator flash =
            Instantiate(
                blastRadiusPrefab,
                logic.transform.position,
                Quaternion.identity
            );

        flash.SetRadius(
            logic.ExplosionRadius
        );

        flash.SetVisible(true);

        Destroy(
            flash.gameObject,
            blastFlashDuration
        );
    }

    private void SpawnShockwave()
    {
        if (shockwavePrefab == null ||
            logic == null)
        {
            return;
        }

        ShockwaveEffect shockwave =
            Instantiate(
                shockwavePrefab,
                logic.transform.position,
                Quaternion.identity
            );

        shockwave.Play(
            logic.ExplosionRadius,
            logic.ShockwaveRadius,
            shockwaveDuration
        );
    }
}