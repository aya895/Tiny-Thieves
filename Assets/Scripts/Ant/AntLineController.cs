using Pathfinding;
using System.Collections.Generic;
using UnityEngine;

public class AntLineController : MonoBehaviour
{
    public List<GameObject> antLine = new List<GameObject>();
    public Transform nest;
    public float spacing = 1.5f;
    public int maxAnts = 10;


    private void OnEnable()
    {
        Ant.OnAntDeath += RemoveAnt;
    }

    private void OnDisable()
    {
        Ant.OnAntDeath -= RemoveAnt;
    }


    void Start()
    {
        UpdatePosition();
    }

    public void UpdatePosition()
    {
        if (antLine != null && antLine.Count > 0)
        {
            // assign target of first ant in line to dessert, others follow
            GameObject dessertObj = GameObject.FindGameObjectWithTag("Dessert");
            if (dessertObj != null && antLine[0] != null)
            {
                AIDestinationSetter destSetter = antLine[0].GetComponent<AIDestinationSetter>();
                if (destSetter != null)
                {
                    destSetter.target = dessertObj.transform;
                }
            }

            for (int i = 1; i < antLine.Count; i++)
            {
                if (antLine[i] != null && antLine[i - 1] != null)
                {
                    AIDestinationSetter destSetter = antLine[i].GetComponent<AIDestinationSetter>();
                    if (destSetter != null)
                    {
                        destSetter.target = antLine[i - 1].transform;
                    }
                }
            }
        }
    }

    public void OnReachedDessert(GameObject ant)
    {
        RemoveAnt(ant);
    }

    public void RemoveAnt(GameObject ant)
    {
        int index = antLine.IndexOf(ant);
        if (index < 0) return;

        antLine.RemoveAt(index);
        UpdatePosition();
    }
}
