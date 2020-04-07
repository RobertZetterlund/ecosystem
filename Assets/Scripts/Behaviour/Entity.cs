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
		return (float)size.GetValue();
	}
	

}
