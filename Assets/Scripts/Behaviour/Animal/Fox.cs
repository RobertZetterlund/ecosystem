using System.Collections;
using UnityEngine;

class Fox : Animal
{
	protected override void Start()
	{
		runAnimationspeedFactor = 0.4f;
		timeToDeathByThirst = simulation.settings.fox.timeToDeathByThirst;
		BiteFactor = simulation.settings.fox.BiteFactor;
		AdultSizeFactor = simulation.settings.fox.AdultSizeFactor;
		lifespan = simulation.settings.fox.lifespan;
		overallCostFactor = simulation.settings.fox.overallCostFactor;
		base.Start();
	}
}