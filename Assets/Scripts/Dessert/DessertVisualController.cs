using UnityEngine;

public class DessertVisualController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Dessert dessert;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Health Sprites")]
    [SerializeField] private Sprite[] healthSprites;

    private int currentSpriteIndex = -1;

    private void Awake()
    {
        ValidateReferences();
    }

    private void OnEnable()
    {
        if (dessert != null)
        {
            dessert.HealthChanged += HandleHealthChanged;
        }
    }

    private void OnDisable()
    {
        if (dessert != null)
        {
            dessert.HealthChanged -= HandleHealthChanged;
        }
    }

    private void Start()
    {
        if (dessert != null)
        {
            HandleHealthChanged(dessert.CurrentHealth, dessert.MaxHealth);
        }
    }

    private void HandleHealthChanged(float currentHealth, float maxHealth)
    {
        if (maxHealth <= 0f || healthSprites.Length == 0)
            return;

        float healthPercentage = currentHealth / maxHealth;

        int spriteIndex = Mathf.FloorToInt(
            (1f - healthPercentage) * healthSprites.Length
        );

        spriteIndex = Mathf.Clamp(
            spriteIndex,
            0,
            healthSprites.Length - 1
        );

        if (spriteIndex == currentSpriteIndex)
            return;

        currentSpriteIndex = spriteIndex;
        spriteRenderer.sprite = healthSprites[spriteIndex];
    }

    private void ValidateReferences()
    {
        if (dessert == null)
        {
            Debug.LogError("[DessertVisualController] Dessert reference is missing.", this);
        }

        if (spriteRenderer == null)
        {
            Debug.LogError("[DessertVisualController] SpriteRenderer reference is missing.", this);
        }

        if (healthSprites == null || healthSprites.Length == 0)
        {
            Debug.LogError("[DessertVisualController] No health sprites assigned.", this);
        }
    }
}