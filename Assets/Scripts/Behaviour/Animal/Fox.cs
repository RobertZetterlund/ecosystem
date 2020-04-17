using System.Collections;
using UnityEngine;

class Fox : Animal
{
	protected override void Start()
	{
		runAnimationspeedFactor = 0.4f;
		base.Start();
	}
}