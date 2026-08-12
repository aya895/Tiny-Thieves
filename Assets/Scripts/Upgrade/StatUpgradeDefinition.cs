using UnityEngine;

// Covers "increase explosion radius," "give one more max TNT," "increase
// knockback," "burn the fuse faster" - all as the SAME class, just
// different asset data. Create one asset per upgrade in the Project
// window; no new code required for any of these (OCP in practice).
[CreateAssetMenu(fileName = "New Stat Upgrade", menuName = "Upgrades/Stat Upgrade")]
public class StatUpgradeDefinition : UpgradeDefinition
{
    [SerializeField] private UpgradeStatType statType;
    [SerializeField] private float amount;

    public override void Apply()
    {
        Debug.Log($"[Upgrade] Apply() called for '{Title}' -> {statType} +{amount}");

        if (PlayerUpgradeStats.Instance == null)
        {
            Debug.LogWarning(
                $"Tried to apply upgrade '{Title}' but no PlayerUpgradeStats exists in the scene. " +
                "Add an empty GameObject with a PlayerUpgradeStats component.");
            return;
        }

        PlayerUpgradeStats.Instance.AddBonus(statType, amount);
    }
}
