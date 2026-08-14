using UnityEngine;

public class AntStackController : MonoBehaviour
{
    private Ant ant;
    private AntMovement antMovement;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;

    public Ant StackedWith { get; private set; }

    private void Awake()
    {
        ant = GetComponent<Ant>();
        antMovement = GetComponent<AntMovement>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!ant.isKnockedBack || StackedWith != null) return;
        Ant other = collision.gameObject.GetComponent<Ant>();
        if (other == null || other == ant) return;

        AntStackController otherStacker = other.GetComponent<AntStackController>();
        if (otherStacker == null || otherStacker.StackedWith != null) return;

        if (other.isKnockedBack && ant.GetInstanceID() < other.GetInstanceID()) return;

        SetStack(other, otherStacker);
    }

    private void SetStack(Ant otherAnt, AntStackController otherStacker)
    {
        StackedWith = otherAnt;
        otherStacker.StackedWith = ant;

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
        transform.SetParent(otherAnt.transform);
        transform.localPosition = new Vector3(0f, 0.3f, 0f);

        if (spriteRenderer != null && otherAnt.GetComponent<SpriteRenderer>() != null)
        {
            spriteRenderer.sortingOrder = otherAnt.GetComponent<SpriteRenderer>().sortingOrder + 1;
        }
    }

    public void LeaveStack()
    {
        if (StackedWith != null)
        {
            // Determine which ant is the passenger (the one that has a parent)
            Ant passenger;
            if(transform.parent != null)
            {
                passenger = ant;
            }
            else
            {
                passenger = StackedWith;
            }

            // Unparent the passenger & restore all its physics & sorting order
            AntStackController passengerStacker = passenger.GetComponent<AntStackController>();
            passenger.transform.SetParent(null);

            Collider2D passengerCollider = passenger.GetComponent<Collider2D>();
            if (passengerCollider != null)
            {
                passengerCollider.enabled = true;
            }

            Rigidbody2D passengerRb = passenger.GetComponent<Rigidbody2D>();
            if (passengerRb != null)
            {
                passengerRb.simulated = true;
            }

            AntMovement passengerMovement = passenger.GetComponent<AntMovement>();
            if (passengerMovement != null)
            {
                passengerMovement.SetPathingEnabled(true);
            }

            SpriteRenderer passengerRenderer = passenger.GetComponent<SpriteRenderer>();
            if (passengerRenderer != null)
            {
                passengerRenderer.sortingOrder = 0;
            }

            if (passengerStacker.StackedWith != null)
            {
                AntStackController otherStacker = passengerStacker.StackedWith.GetComponent<AntStackController>();
                if (otherStacker != null) otherStacker.StackedWith = null; // Clear references
            }

            // Clear references
            passengerStacker.StackedWith = null;
        }
    }
}
