using Pathfinding;
using System.Collections.Generic;
using UnityEngine;

public class AntLineController : MonoBehaviour
{
    public List<GameObject> antLine = new List<GameObject>();
    public Transform nest;
    public float spacing = 1.5f;
    public int maxAnts = 10;

    void Start()
    {
        UpdatePosition();
    }

    public void UpdatePosition()
    {
        if (antLine != null && antLine.Count > 0)
        {
            // assign target of first ant in line to dessrt, others follow
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

    public void OnReachedDessert(GameObject ant)
    {
        antLine.RemoveAt(0);
        UpdatePosition();
    }
}
