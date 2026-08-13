using UnityEngine;

// What each StatUpgradeDefinition is allowed to affect. Keeping this as an
// enum (rather than raw strings/reflection) means a mistyped stat name
// fails at compile time in the Inspector dropdown, not silently at runtime.
public enum UpgradeStatType
{
    ExplosionRadius,
    MaxTNTCount,
    KnockbackForce,
    FuseBurnSpeed,
    MaxFuseDistance,
    MaxDessertHealth
}

// SINGLE RESPONSIBILITY: holds the current total of every upgradeable
// stat and nothing else. It doesn't know what XP is, what a wave is, or
// how upgrades get chosen - it's just the shared answer to "how much
// bonus does the player currently have on X."
//
// TNTLogic and TNTPlacementController read from this; UpgradeDefinitions
// write to it. Neither side needs to know about the other.
public class PlayerUpgradeStats : MonoBehaviour
{
    public static PlayerUpgradeStats Instance { get; private set; }

    public float BonusExplosionRadius { get; private set; }
    public int BonusMaxTNTCount { get; private set; }
    public float BonusKnockbackForce { get; private set; }
    public float BonusFuseBurnSpeed { get; private set; }
    public float BonusMaxFuseDistance { get; private set; }
    public float BonusMaxDessertHealth { get; private set; }

    [SerializeField] private Dessert dessert;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning(
                "A second PlayerUpgradeStats was found in the scene - destroying the duplicate. " +
                "Make sure only ONE GameObject has this component.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void AddBonus(UpgradeStatType type, float amount)
    {
        switch (type)
        {
            case UpgradeStatType.ExplosionRadius:
                BonusExplosionRadius += amount;
                break;
            case UpgradeStatType.MaxTNTCount:
                BonusMaxTNTCount += Mathf.RoundToInt(amount);
                break;
            case UpgradeStatType.KnockbackForce:
                BonusKnockbackForce += amount;
                break;
            case UpgradeStatType.FuseBurnSpeed:
                BonusFuseBurnSpeed += amount;
                break;
            case UpgradeStatType.MaxFuseDistance:
                BonusMaxFuseDistance += amount;
                break;
            case UpgradeStatType.MaxDessertHealth:
                BonusMaxDessertHealth += amount;
                break;
        }

        
        UpgradeAppliedSignal.Raise(type, amount);
    }
}