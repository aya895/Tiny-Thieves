using UnityEngine;

public class WaveMusicController : MonoBehaviour
{
    [Header("Background Music")]
    [SerializeField] private AudioClip planningMusic;
    [SerializeField] private AudioClip gameplayMusic;
    [SerializeField] private AudioClip gameOverMusic;
    [SerializeField] private AudioClip victoryMusic;

    [Header("Countdown SFX")]
    [SerializeField] private AudioClip countdownSound;

    private bool isVictoryMusicPlaying;


    private void OnEnable()
    {
        WaveManager.OnStateChanged += HandleStateChanged;
        WaveManager.OnVictory += HandleVictory;
        WaveManager.OnWaveEnded += HandleWaveEnded;

        WaveCountdownController.OnCountdownSound += HandleCountdownSound;
    }


    private void OnDisable()
    {
        WaveManager.OnStateChanged -= HandleStateChanged;
        WaveManager.OnVictory -= HandleVictory;
        WaveManager.OnWaveEnded -= HandleWaveEnded;

        WaveCountdownController.OnCountdownSound -= HandleCountdownSound;
    }


    private void HandleStateChanged(IWaveState state)
    {
        if (AudioManager.Instance == null)
            return;

        if (state is GameOverState)
        {
            isVictoryMusicPlaying = false;

            AudioManager.Instance.PlayMusic(gameOverMusic);
            return;
        }

        if (state is PlanningState)
        {
            isVictoryMusicPlaying = false;

            AudioManager.Instance.PlayMusic(planningMusic);
            return;
        }

        if (state is PlayingState)
        {
            isVictoryMusicPlaying = false;

            AudioManager.Instance.PlayMusic(gameplayMusic);
            return;
        }

        if (state is UpgradeState)
        {
            if (isVictoryMusicPlaying)
                return;

            AudioManager.Instance.PlayMusic(gameplayMusic);
        }
    }


    private void HandleVictory()
    {
        if (AudioManager.Instance == null)
            return;

        isVictoryMusicPlaying = true;

        AudioManager.Instance.PlayMusic(victoryMusic);
    }


    private void HandleCountdownSound()
    {
        if (AudioManager.Instance == null)
            return;

        AudioManager.Instance.PlayCountdown(
            countdownSound
        );
    }


    private void HandleWaveEnded()
    {
        if (AudioManager.Instance == null)
            return;

        AudioManager.Instance.StopEating();
    }
}