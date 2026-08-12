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
        if (eatingRoutine != null)
            return;

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
            float damage = damagePerSecond * Time.fixedDeltaTime;

            targetDessert.TakeDamage(damage);

            Debug.Log(
                $"[Ant Eating] {gameObject.name} dealt {damage:F2} damage " +
                $"to {targetDessert.name}. " +
                $"Dessert HP: {targetDessert.CurrentHealth:F2}/{targetDessert.MaxHealth:F2}"
            );

            yield return waitForFixedUpdate;
        }

        eatingRoutine = null;
    }

    private void OnDisable()
    {
        StopEating();
        targetDessert = null;
    }
}