using System;

public static class DessertEatingSignal
{
    public static event Action OnEatingStarted;
    public static event Action OnEatingStopped;

    public static void RaiseEatingStarted()
    {
        OnEatingStarted?.Invoke();
    }

    public static void RaiseEatingStopped()
    {
        OnEatingStopped?.Invoke();
    }
}