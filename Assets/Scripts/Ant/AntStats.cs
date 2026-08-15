using UnityEngine;
public class AntStats : MonoBehaviour
{
    [Header("Ant Type")]
    [SerializeField] private AntType antType;

    [Header("Stats")]
    [SerializeField] private float maxHealth = 10f;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private int damageToDessert = 10;

    [Header("TNT")]
    [SerializeField] private float tntResistance = 1f;

    public AntType AntType => antType;
    public float MaxHealth => maxHealth;
    public float MoveSpeed => moveSpeed;
    public int DamageToDessert => damageToDessert;
    public float TntResistance => tntResistance;

    private void OnEnable()
    {
        GameManager.OnMoreAntSpeed += IncreseAntSpeed;
    }
    private void OnDisable()
    {
        GameManager.OnMoreAntSpeed -= IncreseAntSpeed;
    }


    void IncreseAntSpeed()
    {
        moveSpeed++;
    }
}