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
        PlayerUpgradeStats.Instance.AddBonus(statType, amount);
    }
}
