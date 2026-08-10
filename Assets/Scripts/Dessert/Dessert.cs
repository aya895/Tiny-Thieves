using UnityEngine;

public class Dessert : MonoBehaviour
{
    [Header("Dessert Settings")]
    [SerializeField] private float maxHealth = 1000f;

    public float CurrentHealth { get; private set; }

    private void Awake()
    {
        CurrentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (damage <= 0f)
            return;

        CurrentHealth -= damage;
        //Debug.Log(CurrentHealth);

        if (CurrentHealth <= 0f)
        {
            CurrentHealth = 0f;
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Dessert has been destroyed!");

        // Later:
        // WaveManager.Instance.GameOver();
    }
}