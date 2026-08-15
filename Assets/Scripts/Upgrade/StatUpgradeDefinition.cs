using UnityEngine;

[CreateAssetMenu(
    fileName = "New Stat Upgrade",
    menuName = "Upgrades/Stat Upgrade"
)]
public sealed class StatUpgradeDefinition : UpgradeDefinition
{
    [Header("Effect")]
    [SerializeField] private UpgradeStatType statType;

    [SerializeField] private float amount;

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

        context.PlayerStats.AddBonus(
            statType,
            amount
        );
    }
}