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
            Debug.LogError(
                $"Cannot apply '{Title}': UpgradeContext is missing."
            );

            return;
        }

        if (context.PlayerStats == null)
        {
            Debug.LogError(
                $"Cannot apply '{Title}': PlayerUpgradeStats is missing."
            );

            return;
        }

        if (context.Dessert == null)
        {
            Debug.LogError(
                $"Cannot apply '{Title}': Dessert is missing."
            );

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