using System.Collections.Generic;
using UnityEngine;

public class NestPositionCalculate
{
    private float xMin;
    private float xMax;
    private float yMin;
    private float yMax;

    private readonly float minDistanceBetweenNests;
    private readonly float minDistanceFromDessert;

    private readonly float dessertRadius;
    private readonly float nestRadius;

    private readonly Transform dessertTransform;


    public NestPositionCalculate(
        float xMin,
        float xMax,
        float yMin,
        float yMax,
        float minDistanceBetweenNests,
        float minDistanceFromDessert,
        float dessertRadius,
        float nestRadius,
        Transform dessertTransform)
    {
        this.xMin = xMin;
        this.xMax = xMax;
        this.yMin = yMin;
        this.yMax = yMax;

        this.minDistanceBetweenNests = minDistanceBetweenNests;
        this.minDistanceFromDessert = minDistanceFromDessert;

        this.dessertRadius = dessertRadius;
        this.nestRadius = nestRadius;

        this.dessertTransform = dessertTransform;
    }


    public void UpdateArea(
        float xMin,
        float xMax,
        float yMin,
        float yMax)
    {
        this.xMin = xMin;
        this.xMax = xMax;
        this.yMin = yMin;
        this.yMax = yMax;
    }


    public bool TryGetNestPosition(
        List<Vector2> existingPositions,
        out Vector2 position)
    {
        List<Vector2> validPositions =
            GenerateValidPositions(existingPositions);

        if (validPositions.Count == 0)
        {
            position = default;
            return false;
        }

        int randomIndex =
            Random.Range(0, validPositions.Count);

        position = validPositions[randomIndex];

        return true;
    }


    private List<Vector2> GenerateValidPositions(
        List<Vector2> existingPositions)
    {
        List<Vector2> validPositions =
            new List<Vector2>();

        float spacing =
            minDistanceBetweenNests;

        for (float x = xMin;
             x <= xMax;
             x += spacing)
        {
            for (float y = yMin;
                 y <= yMax;
                 y += spacing)
            {
                Vector2 candidate =
                    new Vector2(x, y);

                if (!IsValidPosition(
                        candidate,
                        existingPositions))
                {
                    continue;
                }

                validPositions.Add(candidate);
            }
        }

        return validPositions;
    }


    private bool IsValidPosition(
        Vector2 candidate,
        List<Vector2> existingPositions)
    {
        // =========================================================
        // DESSERT DISTANCE
        // =========================================================

        if (dessertTransform != null)
        {
            float distanceFromDessert =
                Vector2.Distance(
                    candidate,
                    (Vector2)dessertTransform.position
                );

            float requiredDistance =
                dessertRadius +
                nestRadius +
                minDistanceFromDessert;

            if (distanceFromDessert < requiredDistance)
            {
                return false;
            }
        }


        // =========================================================
        // NEST DISTANCE
        // =========================================================

        float minNestDistanceSquared =
            minDistanceBetweenNests *
            minDistanceBetweenNests;

        foreach (Vector2 existingPosition
                 in existingPositions)
        {
            float sqrDistance =
                (candidate - existingPosition).sqrMagnitude;

            if (sqrDistance <
                minNestDistanceSquared)
            {
                return false;
            }
        }


        return true;
    }
}