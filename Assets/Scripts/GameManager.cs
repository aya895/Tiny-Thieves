using System;
using UnityEngine;
using System.Collections;


public class GameManager : MonoBehaviour
{
    public GameManager instance { set; get; }

    private int TimerTillAntArrival = 10;

    // needed events
    public static event Action OnAntArrival;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    void Start() // with waves update the timer will start at each wave play 
    {
        StartCoroutine(WaveTimer(TimerTillAntArrival));
    }

    private IEnumerator WaveTimer(int i)
    {
        yield return new WaitForSeconds(i);
        OnAntArrival?.Invoke();
    }
}
