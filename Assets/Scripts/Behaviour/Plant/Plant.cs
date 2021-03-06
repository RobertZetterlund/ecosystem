﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;


public class Plant : Entity, IConsumable
{
	private RangedDouble amountRemaining;
	private UnityEngine.AI.NavMeshAgent navMeshAgent;
	private bool dead = false;

	public void Init(double size)
	{
		this.species = Species.Plant;
		this.size = new RangedDouble(size, 0);
		amountRemaining = new RangedDouble(size, 0, size);
	}

	// Start is called before the first frame update
	void Start()
	{
		gameObject.tag = "Plant";
		species = Species.Plant;
	}

	[MethodImpl(MethodImplOptions.Synchronized)]
	public double Consume(double amount)
	{

		double consumed = amountRemaining.Add(-amount);

		if (amountRemaining.GetValue() <= 0)
		{
			//Die(CauseOfDeath.Eaten);
			try
			{
				Destroy(gameObject);
				TransformToSappling();
			}
			catch (MissingReferenceException)
			{
				Debug.LogWarning("The animals tried to access a consumable that was already destroyed");
			}

		}

		return consumed;
	}

	bool deleted = false;
	[MethodImpl(MethodImplOptions.Synchronized)]
	private void TransformToSappling()
	{
		if (!deleted)
		{
			deleted = true;
			OrganismFactory.CreateSappling((int)size.GetValue(), transform.position);
		}
	}
	double IConsumable.GetSpeed()
	{
		return 0;
	}

	ConsumptionType IConsumable.GetConsumptionType()
	{
		return ConsumptionType.Plant;
	}

	double IConsumable.GetAmount()
	{
		return amountRemaining.GetValue();
	}
}
