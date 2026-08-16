using TMPro;
using UnityEngine;

public class WaveTimerUI : MonoBehaviour
{
    [SerializeField] private WaveManager waveManager;
    [SerializeField] private TMP_Text timerText;

    private IWaveState currentState;


    private void OnEnable()
    {
        WaveManager.OnStateChanged +=
            HandleStateChanged;
    }


    private void OnDisable()
    {
        WaveManager.OnStateChanged -=
            HandleStateChanged;
    }


    private void HandleStateChanged(IWaveState state)
    {
        currentState = state;

        if (state is UpgradeState ||
            state is GameOverState)
        {
            timerText.text = "";
        }
    }


    private void Update()
    {
        if (waveManager == null ||
            timerText == null)
        {
            return;
        }


        if (currentState is PlanningState)
        {
            timerText.text =
                $"Planning Phase!\n" +
                $"{Mathf.Ceil(waveManager.RemainingTime)}";
        }
        else if (currentState is PlayingState)
        {
            timerText.text =
                $"WAVE {waveManager.CurrentWave}\n" +
                $"{Mathf.Ceil(waveManager.RemainingTime)}";
        }
    }
}