using System;
using UnityEngine;

public abstract class AbstractSensor
{
    public SensorType sensorType;
    public abstract GameObject[] Sense(Transform transform);
}

