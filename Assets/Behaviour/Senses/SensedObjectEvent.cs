using System;
using UnityEngine;
using UnityEngine.AI;

class SensedObjectEvent
{
    Animal sensor { get; }
    GameObject sensed { get; }
    string tag { get; }
    float euclidianDistance;
    float walkingDistance = -1;
    SensorType sensorType { get; }


    public SensedObjectEvent(Animal sensor, GameObject sensed, SensorType sensorType)
    {
        this.sensorType = sensorType;
        this.sensor = sensor;
        this.sensed = sensed;
        tag = sensed.tag;
    }

    public float GetWalkingDistance()
    {
        if (walkingDistance != -1)
        {
            return walkingDistance;
        }
        else
        {
            NavMeshPath path = new NavMeshPath();
            NavMeshAgent agent = sensor.GetComponent<NavMeshAgent>();
            bool canPath = agent.CalculatePath(sensed.transform.position, path);
            if (!canPath)
            {
                walkingDistance = Mathf.Infinity;
            }
            else if (path.corners.Length == 0)
            {
                walkingDistance = euclidianDistance;
            }
            else
            {
                walkingDistance = EnvironmentController.GetPathLength(path);
            }
            return walkingDistance;
            
        }
        
    }

    public float GetEuclidianDistance()
    {
        return (sensed.transform.position - sensor.transform.position).magnitude;
    }
}

