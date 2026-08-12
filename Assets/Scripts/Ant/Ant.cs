using System;
using System.Collections;
using UnityEngine;

// Ant decides for ITSELF whether it was caught in a blast or shockwave -
// TNT never references this class. This is what fixes review comment #2:
// swap Ant for a different enemy type later, or add a Dessert/Player
// listener, without ever opening TNTLogic.cs again.
//
// Requires a Rigidbody2D on this GameObject for the knockback to work -
// if your ants currently move via a script that just sets transform.position
// directly every frame, AddForce won't have any visible effect, since that
// script will immediately overwrite the physics-driven movement. In that
// case either switch ant movement to go through the Rigidbody2D
// (rb.MovePosition / velocity), or skip knockback and just keep the damage
// half of this script.
[RequireComponent(typeof(Rigidbody2D))]
public class Ant : MonoBehaviour, IDamageable
{
    [SerializeField] private float health = 10f;

    [Tooltip("How much XP this ant type grants on death. Vary this per ant prefab variant.")]
    [SerializeField] private float expValue = 5f;

    private Rigidbody2D rb;

    // -----------------------------------------------------------------
    public static event Action <GameObject> OnAntDeath;

    private AntMovement antMovement; 
    [SerializeField] private GameObject stackedVisualPrefab;
    [SerializeField] private float knockBackPathPause = 0.3f;
    [SerializeField] private float xpValue = 1f;
    private SpriteRenderer spriteRenderer;
    private Ant stackedWith;
    private bool isKnockedBack;

    //-------------------------------------------------------------------

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        antMovement = GetComponent<AntMovement>();
        spriteRenderer = GetComponent<SpriteRenderer>();
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

        // Closer to the center = stronger push. Add a small minimum distance
        // so an ant standing exactly on the explosion doesn't divide by ~0.
        float falloff = 1f - Mathf.Clamp01(distance / radius);
        Vector2 direction = distance > 0.01f ? toAnt.normalized : UnityEngine.Random.insideUnitCircle.normalized;

        //rb.AddForce(direction * force * falloff, ForceMode2D.Impulse);
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
        OnAntDeath?.Invoke(this.gameObject);
        AntDeathSignal.Raise(expValue);
        LeaveStack();

        // TODO: death animation / particle / score event here
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isKnockedBack || stackedWith != null) return;
        Ant other = collision.gameObject.GetComponent<Ant>();

        if (other == null || other == this || other.stackedWith != null) return;
        if (other.isKnockedBack == true)
        {
            if (this.GetInstanceID() < other.GetInstanceID())
            {
                return;
            }
        }
        stackedWith = other;
        other.stackedWith = this;

        // Disable physics and pathing on THIS flying ant so it becomes a passenger
        Collider2D antCollider = GetComponent<Collider2D>();

        if (antCollider != null)
        {
            antCollider.enabled = false;
        }
        rb.simulated = false;
        if (antMovement != null)
        {
            antMovement.SetPathingEnabled(false);
        }

        //Parent it to the base ant so it follows its movement exactly & make its sorting order higher
        transform.SetParent(other.transform);
        //
        transform.localPosition = new Vector3(0f, 0.3f, 0f);
        if (spriteRenderer != null && other.spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = other.spriteRenderer.sortingOrder + 1;
        }
    }

    public void LeaveStack()
    {

        if (stackedWith != null)
        {
            // Determine which ant is the passenger (the one that has a parent)
            Ant passenger;
            if (this.transform.parent != null)
            {
                passenger = this;
            }
            else
            {
                passenger = stackedWith;
            }

            // Unparent the passenger & restore its physics & sorting order
            passenger.transform.SetParent(null);
            Collider2D passengerCollider = passenger.GetComponent<Collider2D>();
            if (passengerCollider != null)
            {
                passengerCollider.enabled = true;
            }
            passenger.rb.simulated = true;
            if (passenger.antMovement != null)
            {
                passenger.antMovement.SetPathingEnabled(true);
            }
            if (passenger.spriteRenderer != null)
            {
                passenger.spriteRenderer.sortingOrder = 0;
            }


            // Clear references for both
            stackedWith.stackedWith = null;
            stackedWith = null;
        }
    }
}