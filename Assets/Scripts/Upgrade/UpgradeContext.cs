public sealed class UpgradeContext
{
    public PlayerUpgradeStats PlayerStats { get; }
    public Dessert Dessert { get; }

    public UpgradeContext(
        PlayerUpgradeStats playerStats,
        Dessert dessert)
    {
        PlayerStats = playerStats;
        Dessert = dessert;
    }
}