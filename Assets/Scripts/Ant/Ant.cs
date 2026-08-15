using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Ant : MonoBehaviour, IDamageable
{
    public static event Action<GameObject, float> OnAntDeath;

    [SerializeField] private float health = 10f;
    [SerializeField] private float expValue = 5f;
    [SerializeField] private float knockBackPathPause = 0.3f;

    private Rigidbody2D rb;
    private AntMovement antMovement;
    private AntStackController antStacker;

    public bool isKnockedBack;

    // -----------------------------------------------------------------

    //private SpriteRenderer spriteRenderer;
    //[SerializeField] private float xpValue = 1f;
    //private Ant stackedWith;
    //private bool isKnockedBack;

    //-------------------------------------------------------------------

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        antMovement = GetComponent<AntMovement>();
        //spriteRenderer = GetComponent<SpriteRenderer>();
        antStacker = GetComponent<AntStackController>();
    }

    private void OnEnable()
    {
        ExplosionSignal.OnExplosion += HandleExplosion;
        ShockwaveSignal.OnShockwave += HandleShockwave;
    }

    private void OnDisable()
    {
        ExplosionSignal.OnExplosion -= HandleExplosion;
        ShockwaveSignal.OnShockwave -= HandleShockwave;
    }

    private void HandleExplosion(Vector2 position, float radius, float damage)
    {
        float distance = Vector2.Distance(transform.position, position);
        if (distance <= radius)
        {
            TakeDamage(damage);
        }
    }

    private void HandleShockwave(Vector2 position, float radius, float force)
    {
        Vector2 toAnt = (Vector2)transform.position - position;
        float distance = toAnt.magnitude;

        if (distance > radius) return;
        float falloff = 1f - Mathf.Clamp01(distance / radius);
        Vector2 direction = distance > 0.01f ? toAnt.normalized : UnityEngine.Random.insideUnitCircle.normalized;

        StartCoroutine(ApplyKnockback(direction * force * falloff));
    }

    private IEnumerator ApplyKnockback(Vector2 impulse)
    {
        isKnockedBack = true;
        if (antMovement != null)
        {
            antMovement.SetPathingEnabled(false);
        }
        rb.AddForce(impulse, ForceMode2D.Impulse);

        yield return new WaitForSeconds(knockBackPathPause);

        if (antMovement != null && transform.parent == null)
        {
            antMovement.SetPathingEnabled(true);
        }
        isKnockedBack = false;
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        if (antStacker != null && antStacker.StackedWith != null) // stacked ant give more bonus
        {
            expValue *= 2;
        }
        OnAntDeath?.Invoke(this.gameObject, expValue);

        // TODO: death animation / particle / score event here
        Destroy(gameObject);
    }

    
}