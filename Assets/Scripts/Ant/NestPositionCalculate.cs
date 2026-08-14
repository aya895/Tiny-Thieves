using System.Collections.Generic;
using UnityEngine;

public class NestPositionCalculate
{
    private float xMin, xMax, yMin, yMax;
    private float minDistanceBetweenNests;
    private float minDistanceFromDessert;
    private Transform dessertTransform;

    public NestPositionCalculate(float xMin, float xMax, float yMin, float yMax,
        float minDistanceBetweenNests, float minDistanceFromDessert,
        Transform dessertTransform)
    {
        this.xMin = xMin;
        this.xMax = xMax;
        this.yMin = yMin;
        this.yMax = yMax;
        this.minDistanceBetweenNests = minDistanceBetweenNests;
        this.minDistanceFromDessert = minDistanceFromDessert;
        this.dessertTransform = dessertTransform;
    }

    public void UpdateArea(float xMin, float xMax, float yMin, float yMax)
    {
        this.xMin = xMin;
        this.xMax = xMax;
        this.yMin = yMin;
        this.yMax = yMax;
    }

    public Vector2 GetNestPosition(List<Vector2> existingPositions)
    {
        const int maxAttempts = 50;

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            Vector2 candidate = new Vector2(Random.Range(xMin, xMax), Random.Range(yMin, yMax));

            if (dessertTransform != null && Vector2.Distance(candidate, dessertTransform.position) < minDistanceFromDessert)
                continue;

            bool tooClose = false;
            foreach (Vector2 pos in existingPositions)
            {
                if (Vector2.Distance(candidate, pos) < minDistanceBetweenNests)
                {
                    tooClose = true;
                    break;
                }
            }

            if (tooClose) continue;

            return candidate;
        }
        return new Vector2(Random.Range(xMin, xMax), Random.Range(yMin, yMax));
    }
}
