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

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
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
        Vector2 direction = distance > 0.01f ? toAnt.normalized : Random.insideUnitCircle.normalized;

        rb.AddForce(direction * force * falloff, ForceMode2D.Impulse);
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
        // TODO: death animation / particle / score event here
        Destroy(gameObject);
    }
}