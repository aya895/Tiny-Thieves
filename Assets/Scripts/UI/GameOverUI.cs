using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class GameOverUI : MonoBehaviour
{
    public static event Action OnGameOverShown;
    public static event Action OnGameOverHidden;

    [Header("References")]
    [SerializeField] private WaveManager waveManager;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Button pauseButton;

    [Header("Stats Text")]
    [SerializeField] private TMP_Text clearedWavesText;
    [SerializeField] private TMP_Text reachedWaveText;


    private void Awake()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }


    public void Show()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        if (waveManager != null)
        {
            if (clearedWavesText != null)
            {
                clearedWavesText.text =
                    $"Waves Cleared: {waveManager.ClearedWaves}";
            }

            if (reachedWaveText != null)
            {
                reachedWaveText.text =
                    $"Final Wave: {waveManager.CurrentWave}";
            }
        }

        OnGameOverShown?.Invoke();
    }


    public void RetryWave()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        OnGameOverHidden?.Invoke();

        if (waveManager != null)
        {
            waveManager.ContinueAfterGameOver();
        }
    }


    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}