using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject baseAnt;
    public AntLineController ants;
    private float spawnDelay = 0.75f;

    void Start()
    {
        StartCoroutine(SpawnLine());
    }

    private IEnumerator SpawnLine()
    {

        for (int i = 0; i < ants.maxAnts; i++)
        {
            SpawnAnt();
            yield return new WaitForSeconds(spawnDelay);
        }
    }


    private void SpawnAnt()
    {
        if(ants != null)
        {
            Transform nest = GameObject.FindGameObjectWithTag("Nest").transform;
            Vector2 nestPosition = nest.position;

            GameObject ant = Instantiate(baseAnt, nestPosition, Quaternion.identity);
            if (ant != null)
            {
                ants.antLine.Add(ant);
            }
        }
    }

}
