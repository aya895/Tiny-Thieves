using UnityEngine;

// Unlike StatUpgradeDefinition (fixed amount), this reads Dessert's CURRENT
// MaxHealth at the moment it's applied and adds a percentage of THAT - so
// picking it multiple times compounds (each pick is 15% of the
// already-upgraded total, not 15% of the original base value). This is
// exactly the kind of upgrade that doesn't reduce to "add a flat number,"
// so it gets its own class - still the same UpgradeDefinition contract,
// still interchangeable in the same pool, just its own Strategy.
[CreateAssetMenu(fileName = "New Percent Health Upgrade", menuName = "Upgrades/Percent Max Health Upgrade")]
public class PercentMaxHealthUpgrade : UpgradeDefinition
{
    [SerializeField] [Range(0f, 1f)] private float percent = 0.15f;

    public override void Apply()
    {
        if (Dessert.Instance == null)
        {
            Debug.LogWarning($"Tried to apply upgrade '{Title}' but no Dessert exists in the scene.");
            return;
        }

        if (PlayerUpgradeStats.Instance == null)
        {
            Debug.LogWarning($"Tried to apply upgrade '{Title}' but no PlayerUpgradeStats exists in the scene.");
            return;
        }

        float delta = Dessert.Instance.MaxHealth * percent;
        PlayerUpgradeStats.Instance.AddBonus(UpgradeStatType.MaxDessertHealth, delta);
    }
}
