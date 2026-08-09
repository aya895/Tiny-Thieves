using Pathfinding;
using UnityEngine;

public class AntMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private AIPath aiPath;
    private AIDestinationSetter destinationSetter;
    public AntLineController antLineController;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        aiPath = GetComponent<AIPath>();
        destinationSetter = GetComponent<AIDestinationSetter>();

        // set ai path speed to ant's move speed
        if (aiPath != null)
        {
            aiPath.maxSpeed = GetComponent<AntStats>().MoveSpeed;
            aiPath.canMove = true;
            aiPath.enabled = true;
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Dessert"))
        {
            // notify line controller to remove this ant from line
            antLineController.OnReachedDessert(gameObject);
        }
    }
}

