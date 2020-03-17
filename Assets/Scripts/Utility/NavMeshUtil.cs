using System;
using UnityEngine.AI;
using UnityEngine;

public static class NavMeshUtil
{
    public static Vector3 GetRandomLocation()
    {
        NavMeshTriangulation navMeshData = NavMesh.CalculateTriangulation();

        int maxIndices = navMeshData.indices.Length - 3;
        // Pick the first indice of a random triangle in the nav mesh
        int firstVertexSelected = UnityEngine.Random.Range(0, maxIndices);
        int secondVertexSelected = UnityEngine.Random.Range(0, maxIndices);
        //Spawn on Verticies
        Vector3 point = navMeshData.vertices[navMeshData.indices[firstVertexSelected]];

        Vector3 firstVertexPosition = navMeshData.vertices[navMeshData.indices[firstVertexSelected]];
        Vector3 secondVertexPosition = navMeshData.vertices[navMeshData.indices[secondVertexSelected]];
        //Eliminate points that share a similar X or Z position to stop spawining in square grid line formations
        if ((int)firstVertexPosition.x == (int)secondVertexPosition.x ||
            (int)firstVertexPosition.z == (int)secondVertexPosition.z
            )
        {
            point = GetRandomLocation(); //Re-Roll a position - I'm not happy with this recursion it could be better
        }
        else
        {
            // Select a random point on it
            point = Vector3.Lerp(
                                            firstVertexPosition,
                                            secondVertexPosition, //[t + 1]],
                                            UnityEngine.Random.Range(0.05f, 0.95f) // Not using Random.value as clumps form around Verticies 
                                        );
        }
        //Vector3.Lerp(point, navMeshData.vertices[navMeshData.indices[t + 2]], Random.value); //Made Obsolete

        NavMeshHit myNavHit;
        if (NavMesh.SamplePosition(point, out myNavHit, 100, -1))
        {
            point = myNavHit.position;
        }
        return point;
    }

    public static Vector3 GetRandomLocationV2()
    {
        NavMeshTriangulation navMeshData = NavMesh.CalculateTriangulation();

        // Pick the first indice of a random triangle in the nav mesh
        int t = UnityEngine.Random.Range(0, navMeshData.indices.Length - 3);

        // Select a random point on it
        Vector3 point = Vector3.Lerp(navMeshData.vertices[navMeshData.indices[t]], navMeshData.vertices[navMeshData.indices[t + 1]], UnityEngine.Random.value);
        Vector3.Lerp(point, navMeshData.vertices[navMeshData.indices[t + 2]], UnityEngine.Random.value);

        NavMeshHit myNavHit;
        if (NavMesh.SamplePosition(point, out myNavHit, 100, -1))
        {
            point = myNavHit.position;
        }
        return point;
    }

}