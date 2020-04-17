using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class HuntAction<T> : GoToConsumable<T> where T : IConsumable
{

	public HuntAction(Animal animal) : base(animal)
	{
		approachTimer = new TickTimer(0);
	}

}

