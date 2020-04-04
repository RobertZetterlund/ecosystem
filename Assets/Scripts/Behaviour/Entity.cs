using UnityEngine;
using System;

public abstract class Entity : MonoBehaviour
{

	protected Species species;
	protected RangedDouble size;


	public Species GetSpecies()
	{
		return species;
	}

	public float GetSize()
	{
		Debug.Log("size double " + size.GetValue());
		Debug.Log("size float " + (float)size.GetValue());
		return (float)size.GetValue();
	}

	public static Species GetSpecies(GameObject gameObj)
	{
		foreach(Species species in Enum.GetValues(typeof(Species)))
		{
			if(gameObj.tag.Equals(species.ToString()))
			{
				return species;
			}
		}

		return Species.Undefined;
	}

	public static Entity GetEntity(GameObject gameObj)
	{

		while(!object.ReferenceEquals(gameObj.transform.parent, null))
		{
			gameObj = gameObj.transform.parent.gameObject;
		}
		return (Entity)gameObj.GetComponent( gameObj.gameObject.tag.ToString() );
	}

}
