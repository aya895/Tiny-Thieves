using UnityEngine;

public class WaveMusicController : MonoBehaviour
{
    [Header("Background Music")]
    [SerializeField] private AudioClip planningMusic;
    [SerializeField] private AudioClip gameplayMusic;
    [SerializeField] private AudioClip gameOverMusic;
    [SerializeField] private AudioClip victoryMusic;
    [SerializeField] private AudioClip countdownClip;
    private bool isShowingVictory;


    private void OnEnable()
    {
        WaveManager.OnStateChanged += HandleStateChanged;
        WaveManager.OnVictory += HandleVictory;
        WaveManager.OnCountdownStarted += HandleCountdownStarted;

    }

    private void OnDisable()
    {
        WaveManager.OnStateChanged -= HandleStateChanged;
        WaveManager.OnVictory -= HandleVictory;
        WaveManager.OnCountdownStarted -= HandleCountdownStarted;

    }

    private void HandleCountdownStarted()
    {
        if (AudioManager.Instance == null)
            return;

        AudioManager.Instance.StopMusic();
        AudioManager.Instance.PlaySfx(countdownClip);
    }
    // =========================================================
    // WAVE STATE MUSIC
    // =========================================================

    private void HandleStateChanged(IWaveState state)
    {
        if (AudioManager.Instance == null)
            return;

        if (state is GameOverState)
        {
            isShowingVictory = false;
            PlayMusic(gameOverMusic);
            return;
        }

        // Keep victory music while the Victory panel is visible.
        if (isShowingVictory)
            return;

        if (state is PlanningState)
        {
            PlayMusic(planningMusic);
        }
        else if (state is PlayingState ||
                 state is UpgradeState)
        {
            PlayMusic(gameplayMusic);
        }
    }


    // =========================================================
    // VICTORY
    // =========================================================

    private void HandleVictory()
    {
        isShowingVictory = true;

        PlayMusic(victoryMusic);
    }


    // Call this when the player presses Next on the Victory panel.
    public void FinishVictoryMusic()
    {
        if (!isShowingVictory)
            return;

        isShowingVictory = false;

        PlayMusic(gameplayMusic);
    }


    // =========================================================
    // HELPERS
    // =========================================================

    private void PlayMusic(AudioClip clip)
    {
        if (AudioManager.Instance == null || clip == null)
            return;

        AudioManager.Instance.PlayMusic(clip);
    }
}