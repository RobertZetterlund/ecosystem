using System;
using UnityEngine;

class DynamicPosition : IPosition
{
	Transform transform;
	public DynamicPosition(Transform transform)
	{
		this.transform = transform;
	}
	public Vector3 GetPos()
	{
		return transform.position;
	}
}

