using System;
using UnityEngine;

public class ExperienceManager : MonoBehaviour
{
    [Header("Leveling Curve")]
    [SerializeField] private float baseXPToLevel = 10f;

    [SerializeField] private float xpCurveExponent = 1.3f;

    public int CurrentLevel { get; private set; } = 1;

    public int PendingLevelUps { get; private set; }

    public float CurrentXP { get; private set; }

    public float XPRequiredForNextLevel =>
        CalculateXPRequiredForNextLevel();

    public event Action<float, float, int> XPChanged;

    public event Action UpgradeSelectionRequested;

    public event Action UpgradesResolved;

    private void OnEnable()
    {
        Ant.OnAntDeath += HandleAntDeath;
    }

    private void OnDisable()
    {
        Ant.OnAntDeath -= HandleAntDeath;
    }

    private void HandleAntDeath(
        GameObject ant,
        float expValue)
    {
        AddXP(expValue);
    }

    private void AddXP(float amount)
    {
        if (amount <= 0f)
            return;

        CurrentXP += amount;

        while (
            CurrentXP >=
            CalculateXPRequiredForNextLevel())
        {
            float requiredXP =
                CalculateXPRequiredForNextLevel();

            CurrentXP -= requiredXP;

            CurrentLevel++;

            PendingLevelUps++;
        }

        NotifyXPChanged();
    }

    private float CalculateXPRequiredForNextLevel()
    {
        return baseXPToLevel *
               Mathf.Pow(
                   CurrentLevel,
                   xpCurveExponent
               );
    }

    public void ResolveWaveEnd()
    {
        if (PendingLevelUps > 0)
        {
            UpgradeSelectionRequested?.Invoke();
            return;
        }

        ResetProgress();

        UpgradesResolved?.Invoke();
    }

    public void ConsumePendingLevelUp()
    {
        if (PendingLevelUps <= 0)
        {
            Debug.LogWarning(
                "[ExperienceManager] No pending level-up to consume.",
                this
            );

            return;
        }

        PendingLevelUps--;

        if (PendingLevelUps > 0)
            return;

        ResetProgress();

        UpgradesResolved?.Invoke();
    }

    public void ResetProgress()
    {
        CurrentLevel = 1;
        CurrentXP = 0f;
        PendingLevelUps = 0;

        NotifyXPChanged();
    }

    private void NotifyXPChanged()
    {
        XPChanged?.Invoke(
            CurrentXP,
            XPRequiredForNextLevel,
            CurrentLevel
        );
    }
}