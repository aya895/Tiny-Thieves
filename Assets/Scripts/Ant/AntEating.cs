using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AntStats))]
public sealed class AntEating : MonoBehaviour
{
    private AntStats antStats;
    private Dessert targetDessert;
    private Coroutine eatingRoutine;

    private DessertEatingSound dessertEatingSound;

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

        // Get the ONE shared eating sound from the cake
        dessertEatingSound = dessert.GetComponent<DessertEatingSound>();

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
        dessertEatingSound = null;
    }

    private void StartEating()
    {
        if (eatingRoutine != null)
            return;

        if (targetDessert == null)
            return;

        // Play the shared eating sound
        if (dessertEatingSound != null)
        {
            dessertEatingSound.PlayEatingSound();
        }

        eatingRoutine = StartCoroutine(EatingRoutine());
    }

    private void StopEating()
    {
        if (eatingRoutine == null)
            return;

        StopCoroutine(eatingRoutine);
        eatingRoutine = null;
    }

    private IEnumerator EatingRoutine()
    {
        WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();

        float damagePerSecond = antStats.DamageToDessert;

        while (targetDessert != null)
        {
            if (targetDessert == null)
                break;

            float damage = damagePerSecond * Time.fixedDeltaTime;

            targetDessert.TakeDamage(damage);

            if (targetDessert == null)
                break;

            yield return waitForFixedUpdate;
        }

        eatingRoutine = null;
        targetDessert = null;
        dessertEatingSound = null;
    }

    private void OnDisable()
    {
        StopEating();
        targetDessert = null;
        dessertEatingSound = null;
    }
}