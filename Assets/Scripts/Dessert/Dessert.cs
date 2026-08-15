using System;
using UnityEngine;

public class Dessert : MonoBehaviour, IDamageable
{
    [Header("Dessert")]
    [SerializeField] private float maxHealth = 1000f;

    [Header("Dependencies")]
    [SerializeField]
    private PlayerUpgradeStats playerUpgradeStats;

    public float CurrentHealth { get; private set; }

    public float MaxHealth =>
        maxHealth +
        (playerUpgradeStats != null? playerUpgradeStats.BonusMaxDessertHealth: 0f);

    public event Action<float, float> HealthChanged;

    private void Awake()
    {
        CurrentHealth = MaxHealth;
    }

    private void OnEnable()
    {
        if (playerUpgradeStats != null)
        {
            playerUpgradeStats.BonusApplied += HandleBonusApplied;
        }
    }

    private void OnDisable()
    {
        if (playerUpgradeStats != null)
        {
            playerUpgradeStats.BonusApplied -= HandleBonusApplied;
        }
    }

    public void ResetHealth()
    {
        CurrentHealth = MaxHealth;

        NotifyHealthChanged();
    }

    public void TakeDamage(float damage)
    {
        if (damage <= 0f)
            return;

        if (CurrentHealth <= 0f)
            return;

        CurrentHealth =
            Mathf.Max(
                0f,
                CurrentHealth - damage
            );

        NotifyHealthChanged();

        if (CurrentHealth <= 0f)
        {
            Die();
        }
    }

    private void HandleBonusApplied(
        UpgradeStatType type,
        float amount)
    {
        if (type != UpgradeStatType.MaxDessertHealth)
            return;

        CurrentHealth =
            Mathf.Min(
                CurrentHealth + amount,
                MaxHealth
            );

        NotifyHealthChanged();
    }

    private void NotifyHealthChanged()
    {
        HealthChanged?.Invoke(
            CurrentHealth,
            MaxHealth
        );
    }

    private void Die()
    {
        Debug.Log(
            "[Dessert] Dessert destroyed."
        );

        DessertDestroyedSignal.Raise();
    }
}