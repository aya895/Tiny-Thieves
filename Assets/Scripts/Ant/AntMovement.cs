using UnityEngine;
using Pathfinding;

public class AntMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private AIPath aiPath;
    private AIDestinationSetter destinationSetter;

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

        if(destinationSetter != null)
        {
            var target = GameObject.FindGameObjectWithTag("Dessert");
            if(target != null)
            {
                destinationSetter.target = target.transform;
            }
        }
    }

}
