using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private WaveManager waveManager;
    [SerializeField] private GameObject gameOverPanel;

    private void Awake()
    {
        gameOverPanel.SetActive(false);
    }

    public void Show()
    {
        gameOverPanel.SetActive(true);
    }

    public void NextWave()
    {
        gameOverPanel.SetActive(false);
        waveManager.ContinueAfterGameOver();
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}