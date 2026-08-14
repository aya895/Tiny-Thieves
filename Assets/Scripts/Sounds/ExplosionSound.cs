using UnityEngine;

public class ExplosionSound : MonoBehaviour
{
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private AudioClip explosionClip;

    private void OnEnable()
    {
        ExplosionSignal.OnExplosion += HandleExplosion;
    }

    private void OnDisable()
    {
        ExplosionSignal.OnExplosion -= HandleExplosion;
    }

    private void HandleExplosion(Vector2 position, float radius, float damage)
    {
        //if (audioManager == null || explosionClip == null)
        //    return;

        //audioManager.PlaySfx(explosionClip);
        if (AudioManager.Instance == null || explosionClip == null)
            return;

        AudioManager.Instance.PlaySfx(explosionClip);
    }
}