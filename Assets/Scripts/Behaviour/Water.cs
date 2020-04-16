using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

public class Water : Entity, IConsumable
{
	protected Vector3[] verts;

	// Start is called before the first frame update
	void Start()
	{
		gameObject.tag = "Water";
		species = Species.Water;
		size = new RangedDouble(1f, 0f, 1f);
	}

	public ConsumptionType GetConsumptionType()
	{
		return ConsumptionType.Water;
	}

	double IConsumable.GetAmount()
	{
		return double.MaxValue;
	}

	double IConsumable.Consume(double amount)
	{
		return -amount; // don't need to do anything if we assume there's infinite amount of water
	}

	public double GetSpeed()
	{
		return 0;
	}

	public void SetVerts(Vector3[] newVerts)
	{
		verts = newVerts;
	}

	public Vector3[] GetVerts()
	{
		return verts;
	}

}