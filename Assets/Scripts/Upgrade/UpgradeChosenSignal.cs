using System;

// Raised the moment a player clicks an upgrade choice. Kept separate from
// UpgradeFlowSignal (which means "all pending upgrades are resolved") -
// this fires once per pick, that fires once after the last pick.
public static class UpgradeChosenSignal
{
    public static event Action OnUpgradeChosen;

    public static void Raise()
    {
        OnUpgradeChosen?.Invoke();
    }
}
