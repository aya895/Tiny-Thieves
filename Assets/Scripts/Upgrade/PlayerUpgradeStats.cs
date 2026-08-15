using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUpgradeStats : MonoBehaviour
{
    private readonly Dictionary<UpgradeStatType, float> bonuses =
        new Dictionary<UpgradeStatType, float>();

    public event Action<UpgradeStatType, float> BonusApplied;

    public float BonusExplosionRadius =>
        GetBonus(UpgradeStatType.ExplosionRadius);

    public int BonusMaxTNTCount =>
        Mathf.RoundToInt(
            GetBonus(UpgradeStatType.MaxTNTCount)
        );

    public float BonusKnockbackForce =>
        GetBonus(UpgradeStatType.KnockbackForce);

    public float BonusFuseBurnSpeed =>
        GetBonus(UpgradeStatType.FuseBurnSpeed);

    public float BonusMaxFuseDistance =>
        GetBonus(UpgradeStatType.MaxFuseDistance);

    public float BonusMaxDessertHealth =>
        GetBonus(UpgradeStatType.MaxDessertHealth);

    public void AddBonus(
        UpgradeStatType statType,
        float amount)
    {
        if (Mathf.Approximately(amount, 0f))
            return;

        bonuses[statType] =
            GetBonus(statType) + amount;

        BonusApplied?.Invoke(
            statType,
            amount
        );
    }

    public float GetBonus(UpgradeStatType statType)
    {
        return bonuses.TryGetValue(
            statType,
            out float value
        )
            ? value
            : 0f;
    }

    public void ResetBonuses()
    {
        bonuses.Clear();
    }
}