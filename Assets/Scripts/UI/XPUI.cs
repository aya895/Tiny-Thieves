using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class XPUI : MonoBehaviour
{
    [SerializeField] ExperienceManager experienceManager;

    [SerializeField] Slider xpSlider;

    [SerializeField] TMP_Text levelText;

    private void Start()
    {
        UpdateXP();
    }

    private void Update()
    {
        UpdateXP();
    }

    public void UpdateXP()
    {
        xpSlider.maxValue = experienceManager.baseXPToLevel;
        xpSlider.value = experienceManager.currentXP;
        levelText.text = "Level: " + experienceManager.CurrentLevel;
    }
}
