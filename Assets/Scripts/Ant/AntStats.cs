using UnityEngine;
public class AntStats : MonoBehaviour
{
    [Header("Ant Type")]
    [SerializeField] private AntType antType;

    [Header("Stats")]
    [SerializeField] private float maxHealth = 10f;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private int damageToDessert = 10;
    [SerializeField] private int coinReward = 1;

    [Header("TNT")]
    [SerializeField] private float tntResistance = 1f;

    public AntType AntType => antType;
    public float MaxHealth => maxHealth;
    public float MoveSpeed => moveSpeed;
    public int DamageToDessert => damageToDessert;
    public int CoinReward => coinReward;
    public float TntResistance => tntResistance;
}