using TMPro;
using UnityEngine;

public class WaveTimerUI : MonoBehaviour
{
    [SerializeField] private WaveManager waveManager;
    [SerializeField] private TMP_Text timerText;

    private IWaveState currentState; //just to keep track of state 
    //private bool isCountingDown;
    private float goTimer;


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
        currentState = state;

        if (state is PlayingState)
        {
            goTimer = 1f;
        }
        else if (state is GameOverState)
        {
            timerText.text = "";
        }
    }

    private void Update()
    {
        if (currentState is PlanningState)
        {
            timerText.text = $"Planning Phase!\n{Mathf.Ceil(waveManager.RemainingTime)}";
        }
        else if (currentState is PlayingState)
        {
            if (goTimer > 0f)
            {
                goTimer -= Time.deltaTime;
                timerText.text = "GO!";
                return;
            }
            timerText.text = $"WAVE {waveManager.CurrentWave}\n{Mathf.Ceil(waveManager.RemainingTime)}";
        }
    }
}