using System;
using UnityEngine;

// SINGLE RESPONSIBILITY: XP accumulation and the leveling curve only.
// Doesn't know about ants specifically (just subscribes to AntDeathSignal),
// doesn't know about waves specifically (just subscribes to WaveEndSignal),
// and doesn't know upgrades exist beyond "how many are owed right now."
public class ExperienceManager : MonoBehaviour
{
    [Header("Leveling Curve")]
    [Tooltip("XP required for level 1 -> 2.")]
    public float baseXPToLevel = 10f;
    [Tooltip("Growth rate, Brotato-style: required XP = base * level^exponent. " +
             "Higher = the grind ramps up faster at high levels.")]
    [SerializeField] private float xpCurveExponent = 1.3f;

    public int CurrentLevel { get; private set; } = 0;
    public int PendingLevelUps { get; private set; } = 0;

    public float currentXP;

    // UI or anything else that wants to react to a level-up (e.g. a
    // "LEVEL UP!" popup) can subscribe here without touching wave logic.
    public event Action OnLevelUp;

    private void OnEnable()
    {
        AntDeathSignal.OnAntDied += AddXP;
        WaveEndSignal.OnWaveEnded += HandleWaveEnded;
    }

    private void OnDisable()
    {
        AntDeathSignal.OnAntDied -= AddXP;
        WaveEndSignal.OnWaveEnded -= HandleWaveEnded;
    }

    private void AddXP(float amount)
    {
        currentXP += amount;

        // while, not if - a single big chain reaction can cross more than
        // one threshold at once, and each one owes a separate upgrade choice.
        while (currentXP >= XPRequiredForNextLevel())
        {
            currentXP -= XPRequiredForNextLevel();
            CurrentLevel++;
            PendingLevelUps++;
            OnLevelUp?.Invoke();
        }
    }

    private float XPRequiredForNextLevel()
    {
        return baseXPToLevel * Mathf.Pow(CurrentLevel, xpCurveExponent);
    }

    private void HandleWaveEnded()
    {
        // Nothing earned this wave - resolve immediately so WaveManager
        // can move straight to the next Ready phase without ever knowing
        // upgrades were a possibility this time.
        if (PendingLevelUps <= 0)
        {
            UpgradeFlowSignal.RaiseResolved();
        }

        // If PendingLevelUps > 0, we deliberately do nothing here -
        // UpgradeSelectionUI is also listening for WaveEndSignal and will
        // drive the selection flow, calling ConsumePendingLevelUp() below
        // as the player picks.
    }

    public void ConsumePendingLevelUp()
    {
        PendingLevelUps--;

        if (PendingLevelUps <= 0)
        {
            UpgradeFlowSignal.RaiseResolved();
        }
    }
}
