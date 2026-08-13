using System;

public static class DessertDestroyedSignal
{
    public static event Action OnDessertDestroyed;

    public static void Raise()
    {
        OnDessertDestroyed?.Invoke();
    }
}