using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// SINGLE RESPONSIBILITY: display one upgrade's info and report a click.
// Doesn't know what "applying" an upgrade means or how many are pending -
// that's UpgradeSelectionUI's job.
public class UpgradeChoiceButtonUI : MonoBehaviour
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private Image iconImage;
    [SerializeField] private Button button;

    private UpgradeDefinition upgrade;
    private Action<UpgradeDefinition> onChosen;

    public void Setup(UpgradeDefinition upgradeDefinition, Action<UpgradeDefinition> onChosenCallback)
    {
        

            if (upgradeDefinition == null)
            {
                Debug.LogError("[UpgradeChoiceButtonUI] UpgradeDefinition is NULL.", this);
                return;
            }

            if (titleText == null)
            {
                Debug.LogError("[UpgradeChoiceButtonUI] Title Text is missing.", this);
                return;
            }

            if (descriptionText == null)
            {
                Debug.LogError("[UpgradeChoiceButtonUI] Description Text is missing.", this);
                return;
            }

            if (button == null)
            {
                Debug.LogError("[UpgradeChoiceButtonUI] Button reference is missing.", this);
                return;
            }

            upgrade = upgradeDefinition;
        onChosen = onChosenCallback;

        titleText.text = upgrade.Title;
        descriptionText.text = upgrade.Description;
        if (iconImage != null) iconImage.sprite = upgrade.Icon;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(HandleClick);
    }

    private void HandleClick()
    {
        onChosen?.Invoke(upgrade);
    }
}
