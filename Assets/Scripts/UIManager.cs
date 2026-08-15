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
    public Button pauseButton;
    public TextMeshProUGUI waveClearedAt;
    private StringBuilder builder;
    private bool isPaused = false;

    [Header("Audio Settings")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    private const string MUSIC_PREF_KEY = "MusicVolume";
    private const string SFX_PREF_KEY = "SFXVolume";

    //void Start()
    //{
    //    Time.timeScale = 1f;

    //    builder = new StringBuilder("All ants cleared at wave: ");
    //    waveManager = GetComponent<WaveManager>();
    //    waveClearedAt.text = "";
    //    waveClearedAt.gameObject.SetActive(false);
    //}

    private void Awake()
    {
        Time.timeScale = 1f;
        isPaused = false;

        waveManager = FindFirstObjectByType<WaveManager>();

        builder = new StringBuilder("All ants cleared at wave: ");

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
        //InitVolumeSliders();

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
    }

    private void OnDisable()
    {
        WaveManager.OnVictory -= ShowVictory;

        if (musicSlider != null)
        {
            musicSlider.onValueChanged.RemoveListener(OnMusicVolumeChanged);
        }

        if (sfxSlider != null)
        {
            sfxSlider.onValueChanged.RemoveListener(OnSFXVolumeChanged);
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
        waveClearedAt.gameObject.SetActive(true);


        builder.Append(waveManager.CurrentWave);
        waveClearedAt.text = builder.ToString();
    }

    public void ShowPause()
    {
        if (!isPaused)
        {
            pausePanel.SetActive(true);
            Time.timeScale = 0f; // stops the game entirely
            isPaused = true;
        }
    }

    public void ResumeGame()
    {
        if (isPaused)
        {
            pausePanel.SetActive(false);
            Time.timeScale = 1f;
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
}