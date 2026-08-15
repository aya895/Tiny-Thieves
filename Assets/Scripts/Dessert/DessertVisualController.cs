using System.Collections;
using UnityEngine;

public class DessertVisualController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Dessert dessert;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private BoxCollider2D boxCollider;

    [Header("Health Sprites")]
    [SerializeField] private Sprite[] healthSprites;

    private float iframesDuration = 0.75f;
    private int numOfFlashes = 5;
    private int currentSpriteIndex = -1;
    private float previousHealth = -1f;

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

        int spriteIndex = Mathf.FloorToInt((1f - healthPercentage) * healthSprites.Length);

        spriteIndex = Mathf.Clamp(spriteIndex,0,healthSprites.Length - 1);

        if (spriteIndex == currentSpriteIndex)
            return;

        // i-frames only when taking damage
        bool isFirstSetup = (currentSpriteIndex == -1);
        bool tookDamage = !isFirstSetup && currentHealth < previousHealth;

        previousHealth = currentHealth;

        if (spriteIndex != currentSpriteIndex)
        {
            currentSpriteIndex = spriteIndex;
            spriteRenderer.sprite = healthSprites[spriteIndex];
            UpdateColliderToSprite(healthSprites[spriteIndex]);

        }
        if (tookDamage)
        {
            StopAllCoroutines();
            StartCoroutine(invurnerability());
        }

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

    private void UpdateColliderToSprite(Sprite sprite)
    {
        if (boxCollider != null && sprite != null)
        {
            boxCollider.size = sprite.bounds.size;
            boxCollider.offset = sprite.bounds.center;
        }
    }

    private IEnumerator invurnerability()
    {
        for (int i = 0; i < numOfFlashes; i++)
        {
            spriteRenderer.color = new Color(1, 0, 0, 0.5f);
            yield return new WaitForSeconds(iframesDuration / (numOfFlashes * 2));
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(iframesDuration / (numOfFlashes * 2));
        }
    }
}