using UnityEngine;

public class ExplosionSound : MonoBehaviour
{
    [SerializeField]
    private TNTPlacementController tntController;

    [SerializeField]
    private AudioClip explosionClip;

    private void Awake()
    {
        if (tntController == null)
        {
            tntController =
                FindFirstObjectByType<TNTPlacementController>();
        }
    }

    private void OnEnable()
    {
        if (tntController != null)
        {
            tntController.OnAnyExplosion +=
                HandleExplosion;
        }
    }

    private void OnDisable()
    {
        if (tntController != null)
        {
            tntController.OnAnyExplosion -=
                HandleExplosion;
        }
    }

    private void HandleExplosion(
        Vector2 position,
        float radius,
        float damage)
    {
        if (AudioManager.Instance == null ||
            explosionClip == null)
        {
            return;
        }

        AudioManager.Instance.PlaySfx(
            explosionClip
        );
    }
}