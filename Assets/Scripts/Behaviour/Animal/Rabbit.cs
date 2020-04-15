using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class Rabbit : Animal
{


	protected override void Start()
	{
		runAnimationspeedFactor = 1.3f;
		base.Start();

	}

	protected override bool Immobalize()
	{
		if (base.Immobalize())
		{
			gameObject.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().material = (Material)Resources.Load("red");
			return true;
		}
		return false;
	}
}

