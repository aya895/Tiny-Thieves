using System;
using UnityEngine;

// This is the decoupling layer the review comment asked for.
// TNTLogic calls ExplosionSignal.Raise(...) and moves on - it has NO reference
// to Ant, Dessert, or anything else. Any object that cares about explosions
// (Ant, Dessert, Player shield, a destructible crate...) subscribes on its own.
//
// This means:
//  - TNT can be reused in a totally different scene/game with no ants at all.
//  - Adding a new damageable object never requires touching TNTLogic (Open/Closed).
public static class ExplosionSignal
{
    // position, radius, damage
    public static event Action<Vector2, float, float> OnExplosion;

    public static void Raise(Vector2 position, float radius, float damage)
    {
        OnExplosion?.Invoke(position, radius, damage);
    }
}
