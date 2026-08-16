using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource eatingSource;

    private const string MUSIC_KEY = "MusicVolume";
    private const string SFX_KEY = "SFXVolume";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SetMusicVolume(PlayerPrefs.GetFloat(MUSIC_KEY, 1f));

        SetSFXVolume(PlayerPrefs.GetFloat(SFX_KEY, 1f));
    }

    public void PlayMusic(AudioClip clip)
    {
        if (musicSource == null || clip == null)
            return;

        if (musicSource.clip == clip && musicSource.isPlaying)
        {
            return;
        }

        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void StopMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
            musicSource.clip = null;
        }
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

        if (eatingSource.clip == clip &&
            eatingSource.isPlaying)
        {
            return;
        }

        eatingSource.clip = clip;
        eatingSource.loop = true;
        eatingSource.Play();
    }

    public void StopEating()
    {
        if (eatingSource != null)
        {
            eatingSource.Stop();
        }
    }

    public void SetMusicVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);

        if (musicSource != null)
        {
            musicSource.volume = volume;
        }

        PlayerPrefs.SetFloat(MUSIC_KEY,volume);
    }

    public void SetSFXVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);

        if (sfxSource != null)
        {
            sfxSource.volume = volume;
        }

        if (eatingSource != null)
        {
            eatingSource.volume = volume;
        }

        PlayerPrefs.SetFloat(SFX_KEY,volume);
    }

    public void PauseAll()
    {
        if (musicSource != null && musicSource.isPlaying)
        {
            musicSource.Pause();
        }

        if (sfxSource != null && sfxSource.isPlaying)
        {
            sfxSource.Pause();
        }

        if (eatingSource != null && eatingSource.isPlaying)
        {
            eatingSource.Pause();
        }
    }

    public void ResumeAll()
    {
        if (musicSource != null)
        {
            musicSource.UnPause();
        }

        if (sfxSource != null)
        {
            sfxSource.UnPause();
        }

        if (eatingSource != null)
        {
            eatingSource.UnPause();
        }
    }
}