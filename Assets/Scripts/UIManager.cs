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


    void Start()
    {
        builder = new StringBuilder("All ants cleared at wave: ");
        waveManager = GetComponent<WaveManager>();
        waveClearedAt.text = "";
        waveClearedAt.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        WaveManager.OnVictory += ShowVictory;
    }

    private void OnDisable()
    {
        WaveManager.OnVictory -= ShowVictory;
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
        SceneManager.LoadScene(0);
    }
}