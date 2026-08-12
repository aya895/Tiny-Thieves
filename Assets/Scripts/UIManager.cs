using UnityEngine;

public class UIManager : MonoBehaviour
{
    //public WaveManager waveManager;
    public GameObject victoryPanel;

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
        // stop game and any running thing here (optimization)
        victoryPanel.SetActive(true);
    }

    // more to handle:
    // the pause screen logic
}
