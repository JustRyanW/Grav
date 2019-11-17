using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Calculations {

    public static Rigidbody2D[] attractors;

    public static Vector2 Gravity(Vector2 position, float mass, bool limitMagnitude)
    {
        Vector2 averageAttraction = Vector2.zero;
        Vector2 insideBody = Vector2.zero;
        foreach (Rigidbody2D attractor in attractors)
        {
            Vector2 relativePosition = attractor.position - position;
            float attractionForce = (attractor.mass * mass) / Mathf.Pow(relativePosition.magnitude, 2);
            if (attractionForce > relativePosition.magnitude && limitMagnitude)
            {
                insideBody = relativePosition;
            }
            averageAttraction += attractionForce * relativePosition.normalized;
        }
        if (insideBody != Vector2.zero)
        {
            averageAttraction = insideBody;
        }
        return averageAttraction;
    }

    public static Vector2 Gravity(Vector2 position, float mass)
    {
        return Gravity(position, mass, false);
    }

    public static void Trajectory(Rigidbody2D rigidbody2D, Vector2 origin, Vector2 velcoity, ref LineRenderer lineRenderer, int segmentCount)
    {
        lineRenderer.SetPosition(0, origin);
        float deltaTime = Time.fixedDeltaTime;
        for (int i = 1; i < segmentCount; i++)
        {
            Vector2 lastPosition = lineRenderer.GetPosition(i - 1);
            velcoity += Gravity(lastPosition, rigidbody2D.mass) * deltaTime;
            RaycastHit2D hit = Physics2D.Raycast(lastPosition, velcoity * deltaTime, velcoity.magnitude * deltaTime);
            if (hit.collider != null && hit.rigidbody != rigidbody2D)
            {
                if (i > 5)
                {
                    lineRenderer.enabled = true;
                    segmentCount = i;
                }
                else lineRenderer.enabled = false;
            }
            lineRenderer.SetPosition(i, lastPosition + velcoity * Time.fixedDeltaTime);
            lineRenderer.positionCount = segmentCount;
        }
    }
}
