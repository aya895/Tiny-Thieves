using Pathfinding;
using UnityEngine;

public class AntMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private AIPath aiPath;
    private AIDestinationSetter destinationSetter;
    public AntLineController antLineController;
    private AntStateController stateController;
    private Ant ant;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        aiPath = GetComponent<AIPath>();
        destinationSetter = GetComponent<AIDestinationSetter>();
        stateController = GetComponent<AntStateController>();
        ant = GetComponent<Ant>();

        // set ai path speed to ant's move speed
        if (aiPath != null)
        {
            aiPath.maxSpeed = GetComponent<AntStats>().MoveSpeed;
            SetPathingEnabled(true);
            aiPath.enabled = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Dessert"))
        {
            // notify line controller to remove this ant from line
            antLineController.OnReachedDessert(gameObject);
            if(ant != null)
            {
                ant.LeaveStack();
            }
            stateController.SetState(AntState.Eating); // deft satr da bs al4an ye8yar el state
                                                       // beta3t el ant mn moving le eating 3al4an
                                                       // yeb2a fe tanzem le 7araket el ant:)
        }
    }


    public void SetPathingEnabled(bool enabled)
    {
        if (aiPath != null)
        {
            aiPath.canMove = enabled;
        }
    }

    public void NotifyDeath()
    {
        if (antLineController != null)
        {
            antLineController.RemoveAnt(gameObject);
        }
    }
}

