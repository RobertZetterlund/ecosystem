using System;
using UnityEngine;

public class SearchWaterAction : SearchAction
{
    public SearchWaterAction(Animal animal) : base(animal)
    {

    }

    protected override void Search()
    {
        searchTimer.Tick();
        state = ActionState.Searching;
        if (animal.targetGameObject == null)
        {
            if (searchTimer.IsDone())
            {
                searchTimer.Reset();
                Roam();
            }
        }
        else
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
            targetPos = nearestVertex;
            Approach();
        }
    }
}

