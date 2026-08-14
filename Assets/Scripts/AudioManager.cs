using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource eatingSource;

    public void PlayMusic(AudioClip clip)
    {
        if (musicSource == null || clip == null)
            return;

        if (musicSource.clip == clip && musicSource.isPlaying)
            return;

        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlaySfx(AudioClip clip)
    {
        if (sfxSource == null || clip == null)
            return;

        sfxSource.PlayOneShot(clip);
    }

    public void PlayEating(AudioClip clip)
    {
        if (eatingSource == null || clip == null)
            return;

        if (eatingSource.isPlaying)
            return;

        eatingSource.clip = clip;
        eatingSource.loop = true;
        eatingSource.Play();
    }

    public void StopEating()
    {
        if (eatingSource == null || !eatingSource.isPlaying)
            return;

        eatingSource.Stop();
    }
}