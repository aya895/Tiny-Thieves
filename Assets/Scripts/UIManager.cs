using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Text;

public class UIManager : MonoBehaviour
{
    WaveManager waveManager;
    public GameObject victoryPanel;
    public GameObject pausePanel;
    public GameObject gameCompletePanel;
    public Button pauseButton;
    public Button nextWaveButton;
    public TextMeshProUGUI waveClearedAt;
    public TextMeshProUGUI TotalClearedText;
    private bool isPaused = false;

    [Header("Audio Settings")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    private const string MUSIC_PREF_KEY = "MusicVolume";
    private const string SFX_PREF_KEY = "SFXVolume";

    private void Awake()
    {
        Time.timeScale = 1f;
        isPaused = false;

        waveManager = FindFirstObjectByType<WaveManager>();

        if (waveClearedAt != null)
        {
            waveClearedAt.text = "";
            waveClearedAt.gameObject.SetActive(false);
        }

        if (victoryPanel != null)
        {
            victoryPanel.SetActive(false);
        }

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        InitVolumeSliders();
    }

    private void OnEnable()
    {
        WaveManager.OnVictory += ShowVictory;
        GameOverUI.OnGameOverShown += HidePauseButton;
        GameOverUI.OnGameOverHidden += ShowPauseButton;

        if (musicSlider != null)
        {
            musicSlider.onValueChanged.RemoveListener(OnMusicVolumeChanged);
            musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        }

        if (sfxSlider != null)
        {
            sfxSlider.onValueChanged.RemoveListener(OnSFXVolumeChanged);
            sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        }

        if (nextWaveButton != null)
        {
            nextWaveButton.onClick.RemoveListener(HandleNextWaveClicked);
            nextWaveButton.onClick.AddListener(HandleNextWaveClicked);
        }
    }

    private void OnDisable()
    {
        WaveManager.OnVictory -= ShowVictory;
        GameOverUI.OnGameOverShown -= HidePauseButton;
        GameOverUI.OnGameOverHidden -= ShowPauseButton;

        if (musicSlider != null)
        {
            musicSlider.onValueChanged.RemoveListener(OnMusicVolumeChanged);
        }

        if (sfxSlider != null)
        {
            sfxSlider.onValueChanged.RemoveListener(OnSFXVolumeChanged);
        }

        if (nextWaveButton != null)
        {
            nextWaveButton.onClick.RemoveListener(HandleNextWaveClicked);
        }
    }

    private void InitVolumeSliders()
    {
        float savedMusic = PlayerPrefs.GetFloat(MUSIC_PREF_KEY, 1f);
        float savedSFX = PlayerPrefs.GetFloat(SFX_PREF_KEY, 1f);

        if (musicSlider != null)
        {
            musicSlider.SetValueWithoutNotify(savedMusic); // no event triggers
        }

        if (sfxSlider != null)
        {
            sfxSlider.SetValueWithoutNotify(savedSFX);
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicVolume(savedMusic);
            AudioManager.Instance.SetSFXVolume(savedSFX);
        }
    }

    // Called whenever player moves the pause menu sliders
    private void OnMusicVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicVolume(value);
        }
    }

    private void OnSFXVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetSFXVolume(value);
        }
    }

    private void ShowVictory()
    {
        victoryPanel.SetActive(true);
        Time.timeScale = 0;
        pauseButton.gameObject.SetActive(false);

        if (waveClearedAt != null)
        {
            waveClearedAt.gameObject.SetActive(true);
            waveClearedAt.text = $"Total Waves Cleared: {waveManager.ClearedWaves}";
        }
    }

    private void HandleNextWaveClicked()
    {
        victoryPanel.SetActive(false);

        if (waveClearedAt != null)
        {
            waveClearedAt.gameObject.SetActive(false);
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopEating();
        }

        pauseButton.gameObject.SetActive(true);
        Time.timeScale = 1f;
    }

    //private void ShowGameComplete()
    //{
    //    if (gameCompletePanel != null)
    //    {
    //        gameCompletePanel.SetActive(true);
    //    }

    //    if (TotalClearedText != null && waveManager != null)
    //    {
    //        TotalClearedText.text = $"Total Waves cleared: {waveManager.WavesCleared}";
    //    }

    //    if (pauseButton != null)
    //    {
    //        pauseButton.gameObject.SetActive(false);
    //    }

    //    // Call AudioManager to switch music
    //    if (AudioManager.Instance != null)
    //    {
    //        AudioManager.Instance.PlayGameCompleteMusic();
    //    }

    //    Time.timeScale = 0f;
    //}

    public void ShowPause()
    {
        if (!isPaused)
        {
            pausePanel.SetActive(true);
            Time.timeScale = 0f; // stops the game entirely
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PauseAll();
            }
            isPaused = true;
        }
    }

    public void ResumeGame()
    {
        if (isPaused)
        {
            pausePanel.SetActive(false);
            Time.timeScale = 1f;
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.ResumeAll();
            }
            isPaused = false;
        }
    }

    public void ShowMenu()
    {
        Time.timeScale = 1f;
        isPaused = false;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopEating();
        }

        SceneManager.LoadScene(0);
    }

    private void HidePauseButton()
    {
        if (pauseButton != null)
        {
            pauseButton.gameObject.SetActive(false);
        }
    }
    private void ShowPauseButton()
    {
        if (pauseButton != null)
        {
            pauseButton.gameObject.SetActive(true);
        }
    }
}