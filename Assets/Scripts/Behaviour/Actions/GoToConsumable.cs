using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GoToConsumable<T> : SearchAndAct where T : IConsumable
{
	private TickTimer eatTimer;

	public GoToConsumable(Animal animal) : base(animal)
	{
		eatTimer = new TickTimer(1);
	}

	protected override bool Act()
	{
		if (!base.Act())
			return false;
		IConsumable consumable = animal.targetGameObject.GetComponent<T>();

		eatTimer.Tick();
		if (eatTimer.IsDone())
		{
			animal.Eat(consumable);
			eatTimer.Reset();
			if (consumable == null || consumable.GetAmount() <= 0)
			{
				Reset();
				return true;
			}
		}
		return true;
	}

}

