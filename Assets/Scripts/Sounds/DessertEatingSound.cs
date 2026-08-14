using UnityEngine;

public class DessertEatingSound : MonoBehaviour
{
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private AudioClip eatingClip;

    private void OnEnable()
    {
        DessertEatingSignal.OnEatingStarted += HandleEatingStarted;
        DessertEatingSignal.OnEatingStopped += HandleEatingStopped;
        WaveEndSignal.OnWaveEnded += HandleWaveEnded;
    }

    private void OnDisable()
    {
        DessertEatingSignal.OnEatingStarted -= HandleEatingStarted;
        DessertEatingSignal.OnEatingStopped -= HandleEatingStopped;
        WaveEndSignal.OnWaveEnded -= HandleWaveEnded;
    }

    private void HandleEatingStarted()
    {
        if (audioManager == null || eatingClip == null)
            return;

        audioManager.PlayEating(eatingClip);
    }

    private void HandleEatingStopped()
    {
        if (audioManager == null)
            return;

        audioManager.StopEating();
    }

    private void HandleWaveEnded()
    {
        if (audioManager == null)
            return;

        audioManager.StopEating();
    }
}