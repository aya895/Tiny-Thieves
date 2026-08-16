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
    private readonly Transform dessertTransform;


    public NestPositionCalculate(
        float xMin,
        float xMax,
        float yMin,
        float yMax,
        float minDistanceBetweenNests,
        float minDistanceFromDessert,
        Transform dessertTransform)
    {
        this.xMin = xMin;
        this.xMax = xMax;
        this.yMin = yMin;
        this.yMax = yMax;

        this.minDistanceBetweenNests =
            minDistanceBetweenNests;

        this.minDistanceFromDessert =
            minDistanceFromDessert;

        this.dessertTransform =
            dessertTransform;
    }


    // =========================================================
    // AREA
    // =========================================================

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


    // =========================================================
    // POSITION
    // =========================================================

    public bool TryGetNestPosition(
        List<Vector2> existingPositions,
        out Vector2 position)
    {
        List<Vector2> validPositions =
            GenerateValidPositions(
                existingPositions
            );

        if (validPositions.Count == 0)
        {
            position = default;

            return false;
        }

        int randomIndex =
            Random.Range(
                0,
                validPositions.Count
            );

        position =
            validPositions[randomIndex];

        return true;
    }


    // =========================================================
    // GENERATION
    // =========================================================

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
                    new Vector2(
                        x,
                        y
                    );

                if (!IsValidPosition(
                        candidate,
                        existingPositions))
                {
                    continue;
                }

                validPositions.Add(
                    candidate
                );
            }
        }

        return validPositions;
    }


    // =========================================================
    // VALIDATION
    // =========================================================

    private bool IsValidPosition(
        Vector2 candidate,
        List<Vector2> existingPositions)
    {
        // Keep away from dessert.
        if (dessertTransform != null)
        {
            float sqrDessertDistance =
                (
                    candidate -
                    (Vector2)dessertTransform.position
                ).sqrMagnitude;

            float minDessertDistanceSquared =
                minDistanceFromDessert *
                minDistanceFromDessert;

            if (sqrDessertDistance <
                minDessertDistanceSquared)
            {
                return false;
            }
        }


        // Keep away from other nests.
        float minNestDistanceSquared =
            minDistanceBetweenNests *
            minDistanceBetweenNests;

        foreach (
            Vector2 existingPosition
            in existingPositions)
        {
            float sqrDistance =
                (
                    candidate -
                    existingPosition
                ).sqrMagnitude;

            if (sqrDistance <
                minNestDistanceSquared)
            {
                return false;
            }
        }

        return true;
    }
}