using UnityEngine;

[CreateAssetMenu(
    fileName = "New Percent Health Upgrade",
    menuName = "Upgrades/Percent Max Health Upgrade"
)]
public sealed class PercentMaxHealthUpgrade : UpgradeDefinition
{
    [SerializeField, Range(0f, 1f)]
    private float percent = 0.15f;

    public override void Apply(UpgradeContext context)
    {
        if (context == null)
        {
            return;
        }

        if (context.PlayerStats == null)
        {
            return;
        }

        if (context.Dessert == null)
        {
            return;
        }

        float healthIncrease =
            context.Dessert.MaxHealth * percent;

        context.PlayerStats.AddBonus(
            UpgradeStatType.MaxDessertHealth,
            healthIncrease
        );
    }
}