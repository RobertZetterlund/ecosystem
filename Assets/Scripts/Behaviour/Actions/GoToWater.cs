using System;
using UnityEngine;

public class GoToWater : GoToConsumable<Water>
{
    public GoToWater(Animal animal) : base(animal)
    {

    }


    protected override IPosition PositionToApproach()
    {
        MeshFilter mesh = (MeshFilter)animal.targetGameObject.GetComponent(typeof(MeshFilter));
        float minDistanceSqr = Mathf.Infinity;
        Vector3 nearestVertex = Vector3.zero;
        // scan all vertices to find nearest
        foreach (Vector3 vertex in mesh.sharedMesh.vertices)
        {
            Vector3 diff = animal.transform.position - vertex;
            float distSqr = diff.sqrMagnitude;
            if (distSqr < minDistanceSqr)
            {
                minDistanceSqr = distSqr;
                nearestVertex = vertex;
            }
        }
        return new StaticPosition(nearestVertex);
    }
}

