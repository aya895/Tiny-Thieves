using UnityEngine;

public class UpgradeSound : MonoBehaviour
{
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private AudioClip upgradeClip;

    private void OnEnable()
    {
        UpgradeChosenSignal.OnUpgradeChosen += HandleUpgradeChosen;
    }

    private void OnDisable()
    {
        UpgradeChosenSignal.OnUpgradeChosen -= HandleUpgradeChosen;
    }

    private void HandleUpgradeChosen()
    {
        if (audioManager == null || upgradeClip == null)
            return;

        audioManager.PlaySfx(upgradeClip);
    }
}