using System;
using UnityEngine;

public class Dessert : MonoBehaviour, IDamageable
{
    [Header("Dessert Settings")]
    [SerializeField] private float maxHealth = 1000f;

    public float CurrentHealth { get; private set; }

    // What UI/health bars should read instead of maxHealth directly, since
    // this reflects any MaxDessertHealth upgrades on top of the base value.
    public float MaxHealth =>
        maxHealth + (PlayerUpgradeStats.Instance != null ? PlayerUpgradeStats.Instance.BonusMaxDessertHealth : 0f);
    public event Action<float, float> HealthChanged;
    private void Awake()
    {
        CurrentHealth = MaxHealth;
    }

    private void OnEnable()
    {
        UpgradeAppliedSignal.OnBonusApplied += HandleUpgradeApplied;
    }
 
    private void OnDisable()
    {
        UpgradeAppliedSignal.OnBonusApplied -= HandleUpgradeApplied;
    }

    // MaxHealth is a live calculation, but CurrentHealth is stored state -
    // it needs to be told the instant a health upgrade lands so it can heal
    // by the same amount the cap just increased by (standard roguelite
    // behavior: a +HP upgrade shouldn't just raise an invisible ceiling).
    private void HandleUpgradeApplied(UpgradeStatType type, float amount)
    {
        if (type != UpgradeStatType.MaxDessertHealth) return;
        NotifyHealthChanged();

        CurrentHealth += amount;
        Debug.Log($"[Dessert] Max HP upgrade applied: +{amount}. CurrentHealth is now {CurrentHealth}/{MaxHealth}");
    }

    public void TakeDamage(float damage)
    {
        if (damage <= 0f || CurrentHealth <= 0f)
            return;

        CurrentHealth -= damage;

        if (CurrentHealth <= 0f)
        {
            CurrentHealth = 0f;
            NotifyHealthChanged();
            Die();
            return;
        }

        NotifyHealthChanged();
    }

    private void NotifyHealthChanged()
    {
        HealthChanged?.Invoke(CurrentHealth, MaxHealth);
    }

    private void Die()
    {
        Debug.Log("Dessert has been destroyed!");

        // Later:
        // WaveManager.Instance.GameOver();
    }
}