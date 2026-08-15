using System;

public static class UpgradeChosenSignal
{
    public static event Action OnUpgradeChosen;

    public static void Raise()
    {
        OnUpgradeChosen?.Invoke();
    }
}
