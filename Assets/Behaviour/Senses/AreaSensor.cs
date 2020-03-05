using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Senses an area around the transform of a radius "senseRadius" and transmits the findings to the senseRegistrator
 * 
 */
public class AreaSensor : ISensor
{
    private float senseRadius;

    public AreaSensor(float senseRadius)
    {
        this.senseRadius = senseRadius;
    }

    public GameObject[] Sense(Transform transform)
    {
        Collider[] colliders = EnvironmentController.CheckSurroundings(transform.position, senseRadius);
        GameObject[] sensedGameObjects = new GameObject[colliders.Length];
        for (int i = 0; i < colliders.Length; i++)
        {
            sensedGameObjects[i] = colliders[i].gameObject;
        }
        return sensedGameObjects;
    }
    
}
