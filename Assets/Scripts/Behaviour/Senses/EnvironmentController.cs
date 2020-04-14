using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public static class EnvironmentController
{

	/**
     * returns all colliders that are fully or partly inside a sphere
     * defined by paramters center and radius
     */
	public static Collider[] CheckSurroundings(Vector3 center, float radius)
	{
		return Physics.OverlapSphere(center, radius);
	}

	public static float GetPathLength(NavMeshPath path)
	{
		float lng = 0.0f;

		if ((path.status != NavMeshPathStatus.PathInvalid))
		{
			for (int i = 1; i < path.corners.Length; ++i)
			{
				lng += Vector3.Distance(path.corners[i - 1], path.corners[i]);
			}
		}

		return lng;
	}

}
