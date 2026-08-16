using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    public static event Action OnGameOverShown;
    public static event Action OnGameOverHidden;


    [Header("References")]
    [SerializeField] private WaveManager waveManager;
    [SerializeField] private GameObject gameOverPanel;


    private void Awake()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }


    private void OnEnable()
    {
        WaveManager.OnStateChanged += HandleStateChanged;
    }


    private void OnDisable()
    {
        WaveManager.OnStateChanged -= HandleStateChanged;
    }


    private void HandleStateChanged(IWaveState state)
    {
        if (state is GameOverState)
        {
            Show();
        }
    }


    private void Show()
    {
        if (gameOverPanel == null)
            return;

        gameOverPanel.SetActive(true);

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