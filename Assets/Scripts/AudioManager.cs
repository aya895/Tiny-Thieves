using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    private TNTPlacementController tntController;
    [Header("Clips")]
    [SerializeField] private AudioClip explosionClip;
    [SerializeField] private AudioClip upgradeChosenClip;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        //ExplosionSignal.OnExplosion += HandleExplosion;
        if (tntController == null)
            tntController = FindFirstObjectByType<TNTPlacementController>();

        if (tntController != null)
        {
            tntController.OnAnyExplosion += HandleExplosion;
        }

        UpgradeChosenSignal.OnUpgradeChosen += HandleUpgradeChosen;
    }

    private void OnDisable()
    {
        if (tntController != null)
        {
            tntController.OnAnyExplosion -= HandleExplosion;
        }
        UpgradeChosenSignal.OnUpgradeChosen -= HandleUpgradeChosen;
    }

    // ExplosionSignal's signature is (position, radius, damage) - we only
    // care that an explosion happened, so the params are unused here.
    private void HandleExplosion(Vector2 position, float radius, float damage)
    {
        if (explosionClip != null) audioSource.PlayOneShot(explosionClip);
    }

    private void HandleUpgradeChosen()
    {
        if (upgradeChosenClip != null) audioSource.PlayOneShot(upgradeChosenClip);
    }
}