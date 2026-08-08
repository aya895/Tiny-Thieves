using UnityEngine;

public class AntDamageReceive : MonoBehaviour
{
    
    private AntStats stats;
    private float currentHealth;
    private void Start()
    {
        stats = GetComponent<AntStats>();
        currentHealth = stats.MaxHealth;
    }
    public void TakeTNTDamage(float tntDamage)
    {
        float finalDamage = tntDamage * stats.TntResistance;
        currentHealth -= finalDamage;
        Debug.Log($"{stats.AntType} took {finalDamage} TNT damage. "+$"Current Health: {currentHealth}");
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{stats.AntType} died!");
        Destroy(gameObject);
    }
}
