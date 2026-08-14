using UnityEngine;

public class DessertEatingSound : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] private AudioClip eatingClip;

    [Header("Sound Settings")]
    [SerializeField] private float minPitch = 0.95f;
    [SerializeField] private float maxPitch = 1.05f;

    private void OnEnable()
    {
        WaveEndSignal.OnWaveEnded += StopEatingSound;
    }

    private void OnDisable()
    {
        WaveEndSignal.OnWaveEnded -= StopEatingSound;
    }

    public void PlayEatingSound()
    {
        if (audioSource == null || eatingClip == null)
            return;

        // Don't restart the sound if another ant is already eating.
        if (audioSource.isPlaying)
            return;

        audioSource.pitch = Random.Range(minPitch, maxPitch);
        audioSource.PlayOneShot(eatingClip);
    }

    private void StopEatingSound()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}