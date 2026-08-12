using System;

// Raised by WaveManager when a fresh Ready/Planning phase starts.
// TNTPlacementController listens for this to reset its TNT budget and
// chain-linking state - neither script references the other directly.
public static class WaveReadySignal
{
    public static event Action OnWaveReady;

    public static void Raise()
    {
        OnWaveReady?.Invoke();
    }
}
