using System;
using UnityEngine;

// Deliberately separate from ExplosionSignal (damage). Not every listener
// cares about physics knockback, and not every damageable object has a
// Rigidbody2D - keeping these independent respects Interface Segregation:
// objects only subscribe to the signal they actually need.
public static class ShockwaveSignal
{
    // position, radius, force
    public static event Action<Vector2, float, float> OnShockwave;

    public static void Raise(Vector2 position, float radius, float force)
    {
        OnShockwave?.Invoke(position, radius, force);
    }
}