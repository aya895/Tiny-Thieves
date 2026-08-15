using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class GameOverUI : MonoBehaviour
{
    public static event Action OnGameOverShown;
    public static event Action OnGameOverHidden;

    [Header("References")]
    [SerializeField] private WaveManager waveManager;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Button pauseButton;

    private void Awake()
    {
        gameOverPanel.SetActive(false);
    }

    public void Show()
    {
        gameOverPanel.SetActive(true);
        OnGameOverShown?.Invoke();
    }

    public void RetryWave()
    {
        gameOverPanel.SetActive(false);
        OnGameOverHidden?.Invoke();
        waveManager.ContinueAfterGameOver();
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}