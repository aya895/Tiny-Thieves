using Pathfinding;
using System;
using UnityEngine;

public class AntMovement : MonoBehaviour
{
    public event Action OnDessertReached;

    private Rigidbody2D rb;
    private AIPath aiPath;
    private AIDestinationSetter destinationSetter;
    private Ant ant;
    public AntLineController antLineController;
    private AntStackController stacker;
    private AntStats antStats;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        aiPath = GetComponent<AIPath>();
        destinationSetter = GetComponent<AIDestinationSetter>();
        ant = GetComponent<Ant>();
        stacker = GetComponent<AntStackController>();
        antStats = GetComponent<AntStats>();
    }

    private void OnEnable()
    {
        if (ant != null)
        {
            ant.OnKnockbackStateChanged += HandleKnockbackStateChanged;
        }
        if (stacker != null)
        {
            stacker.OnStackStateChanged += HandleStackStateChanged;
        }

        // set ai path speed to ant's move speed
        if (aiPath != null && antStats != null)
        {
            aiPath.maxSpeed = antStats.MoveSpeed;
            aiPath.enabled = true;
            SetPathingEnabled(true);
        }
    }

    private void OnDisable()
    {
        if (ant != null)
        {
            ant.OnKnockbackStateChanged -= HandleKnockbackStateChanged;
        }
        if (stacker != null)
        {
            stacker.OnStackStateChanged -= HandleStackStateChanged;
        }

        if (destinationSetter != null)
        {
            destinationSetter.target = null;
        }
    }

    private void HandleKnockbackStateChanged(bool isKnockedBack)
    {
        if (isKnockedBack)
        {
            SetPathingEnabled(false);
        }
        // don't resume pathing if it became a stacked passenger mid-knockback
        else if (transform.parent == null)
        {
            SetPathingEnabled(true);
        }
    }

    private void HandleStackStateChanged(bool isPassenger)
    {
        SetPathingEnabled(!isPassenger);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Dessert"))
        {
            // notify line controller to remove this ant from line
            antLineController.OnReachedDessert(gameObject);
            OnDessertReached?.Invoke();
        }
    }

    public void SetPathingEnabled(bool enabled)
    {
        if (aiPath != null)
        {
            aiPath.canMove = enabled;
        }
    }

    public void ResetMovement()
    {
        SetPathingEnabled(true);
        if (destinationSetter != null)
        {
            destinationSetter.target = null;
        }
    }
}

