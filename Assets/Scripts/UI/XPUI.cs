using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class XPUI : MonoBehaviour
{
    [SerializeField]
    private ExperienceManager experienceManager;

    [SerializeField]
    private Slider xpSlider;

    [SerializeField]
    private TMP_Text levelText;

    private void OnEnable()
    {
        if (experienceManager != null)
        {
            experienceManager.XPChanged +=
                HandleXPChanged;
        }
    }

    private void OnDisable()
    {
        if (experienceManager != null)
        {
            experienceManager.XPChanged -=
                HandleXPChanged;
        }
    }

    private void Start()
    {
        if (experienceManager == null)
            return;

        HandleXPChanged(
            experienceManager.CurrentXP,
            experienceManager.XPRequiredForNextLevel,
            experienceManager.CurrentLevel
        );
    }

    private void HandleXPChanged(
        float currentXP,
        float requiredXP,
        int level)
    {
        if (xpSlider != null)
        {
            xpSlider.maxValue = requiredXP;
            xpSlider.value = currentXP;
        }

        if (levelText != null)
        {
            levelText.text =
                $"Level: {level}";
        }
    }
}