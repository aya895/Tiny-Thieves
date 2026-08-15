using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeChoiceButtonUI : MonoBehaviour
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private Image iconImage;
    [SerializeField] private Button button;

    private UpgradeDefinition upgrade;

    private Action<UpgradeDefinition> onChosen;

    public void Setup(
        UpgradeDefinition upgradeDefinition,
        Action<UpgradeDefinition> callback)
    {
        if (upgradeDefinition == null)
        {
            return;
        }

        upgrade = upgradeDefinition;
        onChosen = callback;

        if (titleText != null)
        {
            titleText.text = upgrade.Title;
        }

        if (descriptionText != null)
        {
            descriptionText.text =
                upgrade.Description;
        }

        if (iconImage != null)
        {
            iconImage.sprite = upgrade.Icon;
        }

        if (button != null)
        {
            button.onClick.RemoveListener(
                HandleClick
            );

            button.onClick.AddListener(
                HandleClick
            );
        }
    }

    private void HandleClick()
    {
        onChosen?.Invoke(upgrade);
    }

    private void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(
                HandleClick
            );
        }
    }
}