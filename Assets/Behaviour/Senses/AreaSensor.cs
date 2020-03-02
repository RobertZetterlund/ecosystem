﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaSensor : ISensor
{
    private Transform transform;
    private SenseRegistrator senseRegistrator;
    private float senseRadius;

    public AreaSensor(Transform transform, SenseRegistrator senseRegistrator, float senseRadius)
    {
        this.transform = transform;
        this.senseRegistrator = senseRegistrator;
        this.senseRadius = senseRadius;
    }

    public void Sense()
    {
        Collider[] colliders = EnvironmentController.CheckSurroundings(transform.position, senseRadius);
        foreach (Collider c in colliders)
        {
            GameObject sensedGameObject = c.gameObject;
            senseRegistrator.Register(sensedGameObject);
        }
    }
    
}