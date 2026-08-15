using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Ant : MonoBehaviour, IDamageable, IKnockbackable
{
    public static event Action<GameObject, float> OnAntDeath;
    public event Action<bool> OnKnockbackStateChanged;

    [SerializeField] private float health = 10f;
    [SerializeField] private float expValue = 5f;
    [SerializeField] private float knockBackPathPause = 0.3f;

    private Rigidbody2D rb;
    private AntStackController antStacker;
    private TNTPlacementController tntController;

    public bool isKnockedBack;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        antStacker = GetComponent<AntStackController>();
    }

    private void OnEnable()
    {
        if (tntController == null)
            tntController = FindFirstObjectByType<TNTPlacementController>();

        if (tntController != null)
        {
            tntController.OnAnyExplosion += HandleExplosion;
            tntController.OnAnyShockwave += HandleShockwave;
        }
    }

    private void OnDisable()
    {
        if (tntController != null)
        {
            tntController.OnAnyExplosion -= HandleExplosion;
            tntController.OnAnyShockwave -= HandleShockwave;
        }
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

        StartCoroutine(ApplyKnockbackRoutine(direction * force * falloff));
    }

    public void ApplyKnockback(Vector2 impulse)
    {
        StartCoroutine(ApplyKnockbackRoutine(impulse));
    }

    private IEnumerator ApplyKnockbackRoutine(Vector2 impulse)
    {
        OnKnockbackStateChanged?.Invoke(true);
        rb.AddForce(impulse, ForceMode2D.Impulse);

        yield return new WaitForSeconds(knockBackPathPause);
        OnKnockbackStateChanged?.Invoke(false);
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
        Destroy(gameObject);
    }
}