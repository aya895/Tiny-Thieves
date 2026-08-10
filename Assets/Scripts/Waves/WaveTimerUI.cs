using TMPro;
using UnityEngine;

public class WaveTimerUI : MonoBehaviour
{
    [SerializeField] private WaveManager waveManager;
    [SerializeField] private TMP_Text timerText;

    private bool showGo;
    private float goTimer;

    private void Update()
    {
        if (waveManager.CurrentState == WaveState.Ready)
        {
            showGo = false;

            timerText.text =
                $"Planning Phase!\n{Mathf.Ceil(waveManager.RemainingTime)}";
        }
        else if (waveManager.CurrentState == WaveState.Playing)
        {
            if (!showGo)
            {
                showGo = true;
                goTimer = 1f;
            }

            if (goTimer > 0f)
            {
                goTimer -= Time.deltaTime;
                timerText.text = "GO!";
                return;
            }

            timerText.text =
                $"WAVE {waveManager.CurrentWave}\n" +
                $"{Mathf.Ceil(waveManager.RemainingTime)}";
        }
        else
        {
            timerText.text = "";
        }
    }
}