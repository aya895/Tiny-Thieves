using UnityEngine;

public class AntEating : MonoBehaviour
{
    private AntStats antStats;
    private AntStateController stateController;
    private Dessert targetDessert;

    private void Awake()
    {
        antStats = GetComponent<AntStats>();
        stateController = GetComponent<AntStateController>();
    }

    private void Update()
    {
        if (stateController.CurrentState != AntState.Eating)
            return;

        if (targetDessert == null)
            return;

        targetDessert.TakeDamage(
            antStats.DamageToDessert * Time.deltaTime
        );
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Dessert dessert = collision.gameObject.GetComponent<Dessert>();

        if (dessert == null)
            return;

        targetDessert = dessert;

        stateController.SetState(AntState.Eating);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        Dessert dessert = collision.gameObject.GetComponent<Dessert>();

        if (dessert == null)
            return;

        if (dessert == targetDessert)
        {
            targetDessert = null;

            stateController.SetState(AntState.Moving);
        }
    }
}