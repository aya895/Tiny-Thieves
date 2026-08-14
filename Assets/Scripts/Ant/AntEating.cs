using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AntStats))]
public sealed class AntEating : MonoBehaviour
{
    private AntStats antStats;
    private Dessert targetDessert;
    private Coroutine eatingRoutine;

    private void Awake()
    {
        antStats = GetComponent<AntStats>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.TryGetComponent<Dessert>(out var dessert))
            return;

        if (targetDessert == dessert)
            return;

        targetDessert = dessert;

        StartEating();
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (targetDessert == null)
            return;

        if (!collision.gameObject.TryGetComponent<Dessert>(out var dessert))
            return;

        if (dessert != targetDessert)
            return;

        StopEating();
        targetDessert = null;
    }

    private void StartEating()
    {
        if (eatingRoutine != null || targetDessert == null)
            return;

        DessertEatingSignal.RaiseEatingStarted();

        eatingRoutine = StartCoroutine(EatingRoutine());
    }

    private void StopEating()
    {
        if (eatingRoutine == null)
            return;

        StopCoroutine(eatingRoutine);
        eatingRoutine = null;

        DessertEatingSignal.RaiseEatingStopped();
    }

    private IEnumerator EatingRoutine()
    {
        WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        float damagePerSecond = antStats.DamageToDessert;

        while (targetDessert != null)
        {
            float damage = damagePerSecond * Time.fixedDeltaTime;

            targetDessert.TakeDamage(damage);

            yield return waitForFixedUpdate;
        }

        eatingRoutine = null;
        targetDessert = null;

        DessertEatingSignal.RaiseEatingStopped();
    }

    private void OnDisable()
    {
        StopEating();
        targetDessert = null;
    }
}