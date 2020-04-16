using System;
using System.Collections.Generic;
using UnityEngine;

public class ComponentNavigator
{

	public static Vector3[] rabbitMeshVerts;
	public static Vector3[] foxMeshVerts;
	public static Vector3[] treeMeshVerts;

	public static void LoadData(List<GameObject> puddleList)
	{

		GameObject entity;
		Vector3[] baseVerts;
		Transform renderObjectTransform;

		// The rabbit has it's rotationa and scale values on a different child than the mesh
		entity = (GameObject)Resources.Load("testR");
		rabbitMeshVerts = GetVertsFromBox(entity.GetComponent<BoxCollider>());


		// The fox has it's values on the child with the mesh
		entity = (GameObject)Resources.Load("testF");
		foxMeshVerts = GetVertsFromBox(entity.GetComponent<BoxCollider>());


		// The tree is like the fox and the sappling has the same mesh
		treeMeshVerts = new Vector3[6];
		treeMeshVerts[0] = new Vector3(0, 0.1f, 0.18f);
		treeMeshVerts[1] = new Vector3(0, 2.5f, 0.25f);
		treeMeshVerts[2] = new Vector3(0, 2f, 1f);
		treeMeshVerts[3] = new Vector3(0.5f, 1.7f, -0.33f);
		treeMeshVerts[4] = new Vector3(-0.57f, 1.7f, -0.33f);
		treeMeshVerts[5] = new Vector3(0, 1.7f, -1.28f);

		foreach (GameObject waterpuddle in puddleList)
		{
			((Water)GetEntity(waterpuddle)).SetVerts(waterpuddle.GetComponent<MeshFilter>().sharedMesh.vertices);
		}

	}


	private static Vector3[] GetVerts(Species species, Vector3 position, Vector3 size, Vector3 rotation)
	{

		Vector3[] verts;
		switch (species)
		{
			case Species.Rabbit:
				verts = new Vector3[rabbitMeshVerts.Length];
				for (int i = 0; i < rabbitMeshVerts.Length; i++)
					verts[i] = Quaternion.Euler(rotation) * Vector3.Scale(rabbitMeshVerts[i], size) + position;
				break;
			case Species.Fox:
				verts = new Vector3[foxMeshVerts.Length];
				for (int i = 0; i < foxMeshVerts.Length; i++)
					verts[i] = Quaternion.Euler(rotation) * Vector3.Scale(foxMeshVerts[i], size) + position;
				break;
			case Species.Plant:
				verts = new Vector3[treeMeshVerts.Length];
				for (int i = 0; i < treeMeshVerts.Length; i++)
					verts[i] = Quaternion.Euler(rotation) * Vector3.Scale(treeMeshVerts[i], size) + position;
				break;
			default:
				verts = new Vector3[0];
				break;
		}

		return verts;

	}

	public static Vector3[] GetVerts(Entity entity)
	{

		if (entity.GetSpecies().Equals(Species.Water))
		{
			return ((Water)entity).GetVerts();
		}
		return GetVerts(entity.GetSpecies(), entity.gameObject.transform.position, entity.gameObject.transform.localScale, entity.gameObject.transform.rotation.eulerAngles);
	}


	public static Vector3[] GetVerts(Collider collider)
	{

		Species species = GetSpecies(collider.gameObject);

		return GetVerts(GetEntity(collider.gameObject));

	}

	public static Vector3[] GetVertsFromBox(BoxCollider boxCollider)
	{
		Vector3[] verts = new Vector3[13];
		Vector3 center = boxCollider.center;
		Vector3 size = boxCollider.size/2f;

		int count = 0;
		for(int i = -1; i <= 1; i++)
		{
			for(int j = -1; j <= 1; j++)
			{
				for(int k = -1; k <= 1; k++)
				{
					if( (i + j + k)%2 == 0)
					{
						verts[count] = center + new Vector3(i*size.x, j*size.y, k*size.z);
						count++;
					}
				}
			}
		}
		return verts;
	}

	public static Species GetSpecies(GameObject gameObj)
	{
		gameObj = GoToHighestObject(gameObj);
		foreach (Species species in Enum.GetValues(typeof(Species)))
		{
			if (gameObj.tag.Equals(species.ToString()))
			{
				return species;
			}
		}

		return Species.Undefined;
	}

	public static Entity GetEntity(GameObject gameObj)
	{

		gameObj = GoToHighestObject(gameObj);
		return (Entity)gameObj.GetComponent(gameObj.tag.ToString());
	}

	public static GameObject GoToHighestObject(GameObject gameObj)
	{
		while (!object.ReferenceEquals(gameObj.transform.parent, null))
		{
			gameObj = gameObj.transform.parent.gameObject;
		}
		return gameObj;
	}

	public static Animator GetAnimator(GameObject gameObj)
	{
		return GoToHighestObject(gameObj).GetComponentInChildren<Animator>();

	}

	// Scans all vertices to find nearest
	public static Vector3 GetClosesVert(Vector3 fromPoint, GameObject gameObj)
	{
		if (GetSpecies(gameObj) == Species.Water)
		{
			return GetClosesVert(fromPoint, GetVerts(GetEntity(gameObj)));
		}
		return gameObj.transform.position;
	}

	public static Vector3 GetClosesVert(Vector3 fromPoint, Vector3[] verts)
	{
		float minDistanceSqr = Mathf.Infinity;
		Vector3 nearestVertex = Vector3.zero;
		foreach (Vector3 vertex in verts)
		{
			Vector3 diff = fromPoint - vertex;
			float distSqr = diff.sqrMagnitude;
			if (distSqr < minDistanceSqr)
			{
				minDistanceSqr = distSqr;
				nearestVertex = vertex;
			}
		}
		return nearestVertex;
	}

}
