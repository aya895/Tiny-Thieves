using UnityEngine;
using UnityEngine.Rendering.Universal;

public class AntStats : MonoBehaviour
{
    [Header("Ant Type")]
    [SerializeField] private AntType antType;

    [Header("Stats")]
    [SerializeField] private float maxHealth = 10f;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private int damageToDessert = 1;
    [SerializeField] private int coinReward = 1;

    public AntType AntType => antType;
    public float MaxHealth => maxHealth;
    public float MoveSpeed => moveSpeed;
    public int DamageToDessert => damageToDessert;
    public int CoinReward => coinReward;
}