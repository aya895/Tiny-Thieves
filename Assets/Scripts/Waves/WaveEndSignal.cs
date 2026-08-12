using System;

// WaveManager raises this and moves on with its own business. It doesn't
// know or care who's listening, or what they do about it - that's the
// whole point of the decoupling you asked for.
public static class WaveEndSignal
{
    public static event Action OnWaveEnded;

    public static void Raise()
    {
        OnWaveEnded?.Invoke();
    }
}
