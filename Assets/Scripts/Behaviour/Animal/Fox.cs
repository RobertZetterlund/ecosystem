using System.Collections;
using UnityEngine;

class Fox : Animal
{
	public override void Init(AnimalTraits traits)
	{
		base.Init(traits);
	}
	protected override void Start()
	{
		runAnimationspeedFactor = 1f;
		base.Start();
	}
}