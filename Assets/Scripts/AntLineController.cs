using Pathfinding;
using System.Collections.Generic;
using UnityEngine;

public class AntLineController : MonoBehaviour
{
    public List<GameObject> antLine = new List<GameObject>();
    public float spacing = 1.5f;
    public int maxAnts = 10;

    void Update()
    {
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        if (antLine != null && antLine.Count > 0)
        {
            antLine[0].GetComponent<AIDestinationSetter>().target = GameObject.FindGameObjectWithTag("Dessert").transform;
            for (int i = 1; i < antLine.Count; i++)
            {
                if (antLine[i] != null && antLine[i - 1] != null)
                {
                    antLine[i].GetComponent<AIDestinationSetter>().target = antLine[i - 1].transform;
                }
            }
        }
    }


private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Dessert"))
        {
            // Remove the ant from the line when it reaches the dessert 
            antLine.Remove(antLine[0]);

            // set the dessert to next ant in line 
            if (antLine.Count > 0)
            {
                antLine[0].GetComponent<AIDestinationSetter>().target = GameObject.FindGameObjectWithTag("Dessert").transform;
            }
        }
    }
}
