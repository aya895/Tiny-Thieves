using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource eatingSource;
    [SerializeField] private AudioSource countdownSource;

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

        SetMusicVolume(
            PlayerPrefs.GetFloat(
                MUSIC_KEY,
                1f
            )
        );

        SetSFXVolume(
            PlayerPrefs.GetFloat(
                SFX_KEY,
                1f
            )
        );
    }


    // =========================================================
    // MUSIC
    // =========================================================

    public void PlayMusic(AudioClip clip)
    {
        if (musicSource == null ||
            clip == null)
        {
            return;
        }

        if (musicSource.clip == clip &&
            musicSource.isPlaying)
        {
            return;
        }

        musicSource.Stop();

        musicSource.clip = clip;
        musicSource.loop = true;

        musicSource.Play();
    }


    public void StopMusic()
    {
        if (musicSource == null)
            return;

        musicSource.Stop();
        musicSource.clip = null;
    }


    // =========================================================
    // SFX
    // =========================================================

    public void PlaySfx(AudioClip clip)
    {
        if (sfxSource == null ||
            clip == null)
        {
            return;
        }

        sfxSource.PlayOneShot(clip);
    }


    // =========================================================
    // COUNTDOWN
    // =========================================================

    public void PlayCountdown(AudioClip clip)
    {
        if (countdownSource == null ||
            clip == null)
        {
            return;
        }

        countdownSource.Stop();

        countdownSource.clip = clip;
        countdownSource.loop = false;

        countdownSource.Play();
    }


    public void StopCountdown()
    {
        if (countdownSource == null)
            return;

        countdownSource.Stop();
    }


    // =========================================================
    // EATING
    // =========================================================

    public void PlayEating(AudioClip clip)
    {
        if (eatingSource == null ||
            clip == null)
        {
            return;
        }

        if (eatingSource.clip == clip &&
            eatingSource.isPlaying)
        {
            return;
        }

        eatingSource.Stop();

        eatingSource.clip = clip;
        eatingSource.loop = true;

        eatingSource.Play();
    }


    public void StopEating()
    {
        if (eatingSource == null)
            return;

        eatingSource.Stop();
    }


    // =========================================================
    // VOLUME
    // =========================================================

    public void SetMusicVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);

        if (musicSource != null)
        {
            musicSource.volume = volume;
        }

        PlayerPrefs.SetFloat(
            MUSIC_KEY,
            volume
        );
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

        if (countdownSource != null)
        {
            countdownSource.volume = volume;
        }

        PlayerPrefs.SetFloat(
            SFX_KEY,
            volume
        );
    }


    // =========================================================
    // PAUSE
    // =========================================================

    public void PauseAll()
    {
        musicSource?.Pause();
        sfxSource?.Pause();
        eatingSource?.Pause();
        countdownSource?.Pause();
    }


    public void ResumeAll()
    {
        musicSource?.UnPause();
        sfxSource?.UnPause();
        eatingSource?.UnPause();
        countdownSource?.UnPause();
    }
}