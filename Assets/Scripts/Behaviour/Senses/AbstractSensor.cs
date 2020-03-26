﻿using System;
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

    public abstract void setRadius(float r);
}

