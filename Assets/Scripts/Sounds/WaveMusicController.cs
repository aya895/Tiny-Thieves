using UnityEngine;

public class WaveMusicController : MonoBehaviour
{
    [Header("Background Music")]
    [SerializeField] private AudioClip planningMusic;
    [SerializeField] private AudioClip gameplayMusic;
    [SerializeField] private AudioClip gameOverMusic;
    [SerializeField] private AudioClip victoryMusic;

    private bool isVictoryMusicPlaying;

    private void OnEnable()
    {
        WaveManager.OnStateChanged += HandleStateChanged;
        WaveManager.OnVictory += HandleVictory;
        WaveManager.OnWaveReady += HandleNewWaveReady;
    }

    private void OnDisable()
    {
        WaveManager.OnStateChanged -= HandleStateChanged;
        WaveManager.OnVictory -= HandleVictory;
        WaveManager.OnWaveReady -= HandleNewWaveReady;
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

        // Victory panel is currently active.
        // Do not let UpgradeState replace the victory music.
        if (isVictoryMusicPlaying)
            return;

        if (state is PlanningState)
        {
            AudioManager.Instance.PlayMusic(planningMusic);
        }
        else if (state is PlayingState)
        {
            AudioManager.Instance.PlayMusic(gameplayMusic);
        }
        else if (state is UpgradeState)
        {
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

    private void HandleNewWaveReady()
    {
        isVictoryMusicPlaying = false;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMusic(planningMusic);
        }
    }
}