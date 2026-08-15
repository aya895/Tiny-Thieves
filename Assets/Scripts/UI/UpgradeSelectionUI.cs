using System.Collections.Generic;
using UnityEngine;

public class UpgradeSelectionUI : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private ExperienceManager experienceManager;

    [SerializeField] private PlayerUpgradeStats playerUpgradeStats;

    [SerializeField] private Dessert dessert;

    [Header("Upgrade Pool")]
    [SerializeField]
    private List<UpgradeDefinition> upgradePool;

    [Header("UI")]
    [SerializeField] private GameObject panelRoot;

    [SerializeField] private Transform choicesContainer;

    [SerializeField]
    private UpgradeChoiceButtonUI choiceButtonPrefab;

    [SerializeField] private int choicesPerLevel = 3;

    private readonly List<UpgradeChoiceButtonUI> spawnedButtons =
        new List<UpgradeChoiceButtonUI>();

    private UpgradeContext context;

    private void Awake()
    {
        context = new UpgradeContext(
            playerUpgradeStats,
            dessert
        );

        if (panelRoot != null)
        {
            panelRoot.SetActive(false);
        }
    }

    private void OnEnable()
    {
        if (experienceManager != null)
        {
            experienceManager.UpgradeSelectionRequested +=
                ShowNextChoice;
        }
    }

    private void OnDisable()
    {
        if (experienceManager != null)
        {
            experienceManager.UpgradeSelectionRequested -=
                ShowNextChoice;
        }
    }

    private void ShowNextChoice()
    {
        if (experienceManager == null)
            return;

        if (experienceManager.PendingLevelUps <= 0)
            return;

        if (panelRoot == null)
            return;

        panelRoot.SetActive(true);

        ClearButtons();

        List<UpgradeDefinition> choices =
            PickRandomUpgrades(choicesPerLevel);

        foreach (UpgradeDefinition upgrade in choices)
        {
            UpgradeChoiceButtonUI button =
                Instantiate(
                    choiceButtonPrefab,
                    choicesContainer
                );

            button.Setup(
                upgrade,
                HandleUpgradeChosen
            );

            spawnedButtons.Add(button);
        }
    }

    private void HandleUpgradeChosen(
        UpgradeDefinition chosen)
    {
        if (chosen == null)
            return;

        chosen.Apply(context);

        UpgradeChosenSignal.Raise();

        experienceManager.ConsumePendingLevelUp();

        if (experienceManager.PendingLevelUps > 0)
        {
            ShowNextChoice();
            return;
        }

        panelRoot.SetActive(false);

        ClearButtons();
    }

    private List<UpgradeDefinition> PickRandomUpgrades(
        int count)
    {
        List<UpgradeDefinition> available =
            new List<UpgradeDefinition>();

        foreach (UpgradeDefinition upgrade in upgradePool)
        {
            if (upgrade != null)
            {
                available.Add(upgrade);
            }
        }

        List<UpgradeDefinition> selected =
            new List<UpgradeDefinition>();

        int amount =
            Mathf.Min(
                count,
                available.Count
            );

        for (int i = 0; i < amount; i++)
        {
            int index =
                Random.Range(
                    0,
                    available.Count
                );

            selected.Add(
                available[index]
            );

            available.RemoveAt(index);
        }

        return selected;
    }

    private void ClearButtons()
    {
        foreach (UpgradeChoiceButtonUI button in spawnedButtons)
        {
            if (button != null)
            {
                Destroy(button.gameObject);
            }
        }

        spawnedButtons.Clear();
    }
}