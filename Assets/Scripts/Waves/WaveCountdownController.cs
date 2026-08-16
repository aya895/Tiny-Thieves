using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class WaveCountdownController : MonoBehaviour
{
    public static event Action OnCountdownSound;
    public static event Action OnCountdownFinished;


    [Header("Countdown")]
    [SerializeField] private TextMeshProUGUI countdownText;

    [SerializeField] private int countdownStartNumber = 3;

    [SerializeField] private float goDuration = 0.75f;


    private const float CountdownStepDuration = 1f;

    private Coroutine countdownCoroutine;


    private void Awake()
    {
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(false);
        }
    }


    public void StartCountdown()
    {
        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine);
        }

        countdownCoroutine =
            StartCoroutine(CountdownRoutine());
    }


    private IEnumerator CountdownRoutine()
    {
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(true);
        }


        for (int number = countdownStartNumber;
             number > 0;
             number--)
        {
            if (countdownText != null)
            {
                countdownText.text =
                    number.ToString();
            }

            OnCountdownSound?.Invoke();

            yield return new WaitForSeconds(
                CountdownStepDuration
            );
        }


        if (countdownText != null)
        {
            countdownText.text = "GO!";
        }

        OnCountdownSound?.Invoke();


        yield return new WaitForSeconds(
            goDuration
        );


        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(false);
        }


        countdownCoroutine = null;

        OnCountdownFinished?.Invoke();
    }
}