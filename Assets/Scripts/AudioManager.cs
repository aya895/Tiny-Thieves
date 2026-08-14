using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private TNTPlacementController tntController;
    [Header("Clips")]
    [SerializeField] private AudioClip explosionClip;
    [SerializeField] private AudioClip upgradeChosenClip;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource eatingSource;

    private const string MUSIC_KEY = "MusicVolume";
    private const string SFX_KEY = "SFXVolume";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // load changed music & sfx
        SetMusicVolume(PlayerPrefs.GetFloat(MUSIC_KEY, 1f));
        SetSFXVolume(PlayerPrefs.GetFloat(SFX_KEY, 1f));
    }

    public void SetMusicVolume(float volume)
    {
        if (musicSource != null)
        {
            musicSource.volume = volume;
        }
        PlayerPrefs.SetFloat(MUSIC_KEY, volume);
    }

    public void SetSFXVolume(float volume)
    {
        if (sfxSource != null)
        {
            sfxSource.volume = volume;
        }
        if (eatingSource != null)
        {
            eatingSource.volume = volume;
        }
        PlayerPrefs.SetFloat(SFX_KEY, volume);
    }

    //public void SetVolume(float volume) // called whenever volume changes :)
    //{
    //    if (audioSource != null)
    //    {
    //        audioSource.volume = volume;
    //    }
    //    PlayerPrefs.SetFloat("MusicVolume", volume);
    //}

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
        //ExplosionSignal.OnExplosion += HandleExplosion;
        if (tntController == null)
            tntController = FindFirstObjectByType<TNTPlacementController>();

        if (tntController != null)
        {
            tntController.OnAnyExplosion += HandleExplosion;
        }

        UpgradeChosenSignal.OnUpgradeChosen += HandleUpgradeChosen;
    }

    public void PlayEating(AudioClip clip)
    {
        if (tntController != null)
        {
            tntController.OnAnyExplosion -= HandleExplosion;
        }
        UpgradeChosenSignal.OnUpgradeChosen -= HandleUpgradeChosen;
    }

    public void StopEating()
    {
        if (eatingSource != null && eatingSource.isPlaying)
        {
            eatingSource.Stop();
        }
    }

    public void StopMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }
}