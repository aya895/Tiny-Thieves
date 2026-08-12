using System;

// The other half of the handshake. Whoever decides upgrades are "done"
// (zero pending, or the player just finished picking the last one) raises
// this. WaveManager subscribes to it and has no idea whether it fired
// because there was nothing to upgrade, or because five choices just got made.
public static class UpgradeFlowSignal
{
    public static event Action OnResolved;

    public static void RaiseResolved()
    {
        OnResolved?.Invoke();
    }
}
