using System;
using UnityEngine;

public abstract class AbstractSensor
{
	public SensorType sensorType;
	public abstract GameObject[] Sense(Transform transform);
	public bool IsSensingTag(Transform transform, string gametag)
	{
		foreach (GameObject gameObject in Sense(transform))
		{
			if (!gametag.Equals("") && gameObject.CompareTag(gametag) && gameObject.transform != transform)
			{
				return true;
			}
		}
		return false;
	}

	public bool IsSensingObject(Transform transform, GameObject targetGameObject)
	{
		foreach (GameObject gameObject in Sense(transform))
		{
			if (targetGameObject != null && gameObject.Equals(targetGameObject))
			{
				return true;
			}
		}
		return false;
	}
	public abstract void SetRadius(float r);
	public abstract float GetRadius();
}

