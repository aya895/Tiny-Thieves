using System;

// Most upgrade effects (explosion radius, fuse speed, etc.) are read
// on-demand via properties, so they don't need this. But some things -
// like the Dessert's current HP - are stored STATE, not a live calculation,
// and need to be told the instant a relevant upgrade lands (e.g. to heal
// by the same amount the max HP just increased by).
public static class UpgradeAppliedSignal
{
    public static event Action<UpgradeStatType, float> OnBonusApplied; // type, amount added (delta, not total)

    public static void Raise(UpgradeStatType type, float amount)
    {
        OnBonusApplied?.Invoke(type, amount);
    }
}
