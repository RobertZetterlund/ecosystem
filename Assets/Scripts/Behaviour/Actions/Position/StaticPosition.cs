using System;
using UnityEngine;

public class StaticPosition : IPosition
{
	Vector3 pos;
	public StaticPosition(Vector3 pos)
	{
		this.pos = pos;
	}
	public Vector3 GetPos()
	{
		return pos;
	}
}

