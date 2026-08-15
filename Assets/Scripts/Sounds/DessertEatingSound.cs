using UnityEngine;

public class DessertEatingSound : MonoBehaviour
{
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private AudioClip eatingClip;

    private void OnEnable()
    {
        DessertEatingSignal.OnEatingStarted += HandleEatingStarted;
        DessertEatingSignal.OnEatingStopped += HandleEatingStopped;
        WaveManager.OnWaveEnded += HandleWaveEnded;
    }

    private void OnDisable()
    {
        DessertEatingSignal.OnEatingStarted -= HandleEatingStarted;
        DessertEatingSignal.OnEatingStopped -= HandleEatingStopped;
        WaveManager.OnWaveEnded -= HandleWaveEnded;
    }

    private void HandleEatingStarted()
    {
        if (AudioManager.Instance == null || eatingClip == null)
            return;

        AudioManager.Instance.PlayEating(eatingClip);
    }

    private void HandleEatingStopped()
    {
        if (AudioManager.Instance == null)
            return;

        AudioManager.Instance.StopEating();
    }

    private void HandleWaveEnded()
    {
        if (AudioManager.Instance == null)
            return;

        AudioManager.Instance.StopEating();
    }
}