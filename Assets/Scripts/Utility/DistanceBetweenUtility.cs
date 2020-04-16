using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public static class DistanceBetweenUtility
{
	public static float DistanceBetweenTwoGameObjects(GameObject obj1, GameObject obj2)
	{
		if(obj1 == null || obj2 == null) {
			return Int32.MaxValue;
		}
		if(ComponentNavigator.GetSpecies(obj1) == Species.Water) {
			return DistanceToWater(obj2, obj1);
		} else if(ComponentNavigator.GetSpecies(obj2) == Species.Water) {
			return DistanceToWater(obj1, obj2);
		}

		return Vector3.Distance(obj1.transform.position, obj2.transform.position);
	}

	private static float DistanceToWater(GameObject obj, GameObject waterObj) {
		Vector3 closestVert = GetClosesVert(obj.transform.position, waterObj);
		return Vector3.Distance(obj.transform.position, closestVert);
	}

	public static Vector3 GetClosesVert(Vector3 fromPoint, GameObject gameObj) {
		if(ComponentNavigator.GetSpecies(gameObj) == Species.Water) {
			return GetClosesVert(fromPoint, ComponentNavigator.GetVerts(ComponentNavigator.GetEntity(gameObj)));
		}
		return gameObj.transform.position;
	}

	public static Vector3 GetClosesVert(Vector3 fromPoint, Vector3[] verts) {
		float minDistanceSqr = Mathf.Infinity;
		Vector3 nearestVertex = Vector3.zero;
		foreach(Vector3 vertex in verts) {
			Vector3 diff = fromPoint - vertex;
			float distSqr = diff.sqrMagnitude;
			if(distSqr < minDistanceSqr) {
				minDistanceSqr = distSqr;
				nearestVertex = vertex;
			}
		}
		return nearestVertex;
	}

}
