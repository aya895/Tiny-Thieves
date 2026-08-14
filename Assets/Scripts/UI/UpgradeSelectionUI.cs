using System.Collections.Generic;
using UnityEngine;

// SINGLE RESPONSIBILITY: presentation and selection flow only. Applying an
// upgrade's effect is the upgrade's own job (Apply()); tracking XP/levels
// is ExperienceManager's job; this class just shows choices and reacts to clicks.
public class UpgradeSelectionUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ExperienceManager experienceManager;
    [SerializeField] private List<UpgradeDefinition> upgradePool;

    [Header("UI")]
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private Transform choicesContainer;
    [SerializeField] private UpgradeChoiceButtonUI choiceButtonPrefab;
    [SerializeField] private int choicesPerLevel = 3;

    private readonly List<UpgradeChoiceButtonUI> spawnedButtons = new List<UpgradeChoiceButtonUI>();
    
    private void OnEnable()
    {
        WaveEndSignal.OnWaveEnded += HandleWaveEnded;
    }

    private void OnDisable()
    {
        WaveEndSignal.OnWaveEnded -= HandleWaveEnded;
    }

    private void HandleWaveEnded()
    {
        // If nothing is pending, ExperienceManager already resolved things
        // itself - this UI has nothing to do and stays hidden.
        if (experienceManager != null &&
        experienceManager.PendingLevelUps > 0)
        {
            ShowNextChoice();
        }

    }
    public void ShowUpgrade()
    {
        if (experienceManager == null ||
            experienceManager.PendingLevelUps <= 0)
        {
            return;
        }

        ShowNextChoice();
    }
    private void ShowNextChoice()
    {
        panelRoot.SetActive(true);
        ClearButtons();

        foreach (UpgradeDefinition upgrade in PickRandomUpgrades(choicesPerLevel))
        {
            UpgradeChoiceButtonUI buttonUI = Instantiate(choiceButtonPrefab, choicesContainer);
            buttonUI.Setup(upgrade, HandleUpgradeChosen);
            spawnedButtons.Add(buttonUI);
        }
    }

    private void HandleUpgradeChosen(UpgradeDefinition chosen)
    {
        chosen.Apply();
        UpgradeChosenSignal.Raise();
        experienceManager.ConsumePendingLevelUp();

        if (experienceManager.PendingLevelUps > 0)
        {
            // Another level was banked from the same wave - immediately
            // show the next choice instead of closing the panel.
            ShowNextChoice();
        }
        else
        {
            panelRoot.SetActive(false);
            ClearButtons();
            // No need to raise UpgradeFlowSignal here - ConsumePendingLevelUp()
            // already did, since ExperienceManager owns that responsibility.
        }
    }

    private void ClearButtons()
    {
        foreach (UpgradeChoiceButtonUI b in spawnedButtons)
        {
            Destroy(b.gameObject);
        }
        spawnedButtons.Clear();
    }

    private List<UpgradeDefinition> PickRandomUpgrades(int count)
    {
        List<UpgradeDefinition> pool = new List<UpgradeDefinition>(upgradePool);
        List<UpgradeDefinition> result = new List<UpgradeDefinition>();

        int pickCount = Mathf.Min(count, pool.Count);
        for (int i = 0; i < pickCount; i++)
        {
            int index = Random.Range(0, pool.Count);
            result.Add(pool[index]);
            pool.RemoveAt(index); // no duplicate choices within the same prompt
        }

        return result;
    }
}
