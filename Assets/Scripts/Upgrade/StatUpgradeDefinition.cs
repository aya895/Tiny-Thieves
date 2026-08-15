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
            return;
        }

        if (context.PlayerStats == null)
        {
            return;
        }

        context.PlayerStats.AddBonus(
            statType,
            amount
        );
    }
}