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

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (!isKnockedBack || stackedWith != null) return;
    //    Ant other = collision.gameObject.GetComponent<Ant>();

    //    if (other == null || other == this || other.stackedWith != null) return;
    //    if (other.isKnockedBack == true)
    //    {
    //        if (this.GetInstanceID() < other.GetInstanceID())
    //        {
    //            return;
    //        }
    //    }
    //    stackedWith = other;
    //    other.stackedWith = this;

    //    // Disable physics and pathing on THIS flying ant so it becomes a passenger
    //    Collider2D antCollider = GetComponent<Collider2D>();

    //    if (antCollider != null)
    //    {
    //        antCollider.enabled = false;
    //    }
    //    rb.simulated = false;
    //    if (antMovement != null)
    //    {
    //        antMovement.SetPathingEnabled(false);
    //    }

    //    //Parent it to the base ant so it follows its movement exactly & make its sorting order higher
    //    transform.SetParent(other.transform);
    //    //
    //    transform.localPosition = new Vector3(0f, 0.3f, 0f);
    //    if (spriteRenderer != null && other.spriteRenderer != null)
    //    {
    //        spriteRenderer.sortingOrder = other.spriteRenderer.sortingOrder + 1;
    //    }
    //}

    //public void LeaveStack()
    //{

    //    if (stackedWith != null)
    //    {
    //        // Determine which ant is the passenger (the one that has a parent)
    //        Ant passenger;
    //        if (this.transform.parent != null)
    //        {
    //            passenger = this;
    //        }
    //        else
    //        {
    //            passenger = stackedWith;
    //        }

    //        // Unparent the passenger & restore its physics & sorting order
    //        passenger.transform.SetParent(null);
    //        Collider2D passengerCollider = passenger.GetComponent<Collider2D>();
    //        if (passengerCollider != null)
    //        {
    //            passengerCollider.enabled = true;
    //        }
    //        passenger.rb.simulated = true;
    //        if (passenger.antMovement != null)
    //        {
    //            passenger.antMovement.SetPathingEnabled(true);
    //        }
    //        if (passenger.spriteRenderer != null)
    //        {
    //            passenger.spriteRenderer.sortingOrder = 0;
    //        }


    //        // Clear references for both
    //        stackedWith.stackedWith = null;
    //        stackedWith = null;
    //    }
    //}
}