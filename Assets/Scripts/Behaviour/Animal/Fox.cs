using System.Collections;
using UnityEngine;

class Fox : Animal
{
	protected override void Start()
	{
		runAnimationspeedFactor = 1f;
		base.Start();
	}
}