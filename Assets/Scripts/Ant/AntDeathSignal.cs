using System;

// Same pattern as ExplosionSignal/ShockwaveSignal: Ant announces its death
// and how much XP it's worth, without knowing anything about leveling,
// upgrades, or who's listening.
public static class AntDeathSignal
{
    public static event Action<float> OnAntDied; // expValue

    public static void Raise(float expValue)
    {
        OnAntDied?.Invoke(expValue);
    }
}
