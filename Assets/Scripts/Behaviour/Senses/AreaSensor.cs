using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Senses an area around the transform of a radius "senseRadius" and returns the results if
 * they happen so be within the specifiedd horisontal and vertical field. Blockable is a toggle
 * which means they game objects wont be registered if they are behind a wall of instance.
 * 
 */
public class AreaSensor : AbstractSensor
{
    private float senseRadius, fovHorisontal, fovVertical;
    private bool blockable;

    public AreaSensor(float senseRadius, float fovHorisontal, float fovVertical, bool blockable, SensorType sensorType)
    {
        this.senseRadius = senseRadius;
        this.fovHorisontal = fovHorisontal;
        this.fovVertical = fovVertical;
        this.blockable = blockable;
        this.sensorType = sensorType;
    }

    public override GameObject[] Sense(Transform transform)
    {
        Collider[] colliders = EnvironmentController.CheckSurroundings(transform.position, senseRadius);
        List<GameObject> sensedGameObjects = new List<GameObject>();
        for (int i = 0; i < colliders.Length; i++)
        {

            GameObject sensedObject = colliders[i].gameObject;

            Vector3 dir;
            Vector3 forward;

            //Calculate if the object is within the horisontal field of view

            //Calculate the direction from the host transform to the target transform and project it on
            //the plane that the host is walking on
            dir = sensedObject.transform.position - transform.position;
            dir = Vector3.ProjectOnPlane(dir, transform.up);
            dir = Vector3.Normalize(dir);

            //Calculate the direction of the host.
            forward = Vector3.ProjectOnPlane(transform.forward, transform.up);
            forward = Vector3.Normalize(forward);
            //The dot product between the two direction is equal to the angle between the vectors.
            float horisontalAngle = Mathf.Rad2Deg * Mathf.Acos(Vector3.Dot(forward, dir));

            //Continue with calculations if horisontal angle is OK
            if(horisontalAngle > fovHorisontal / 2)
            {
                continue;
            }

            //Same as previously but now we are instead projecting everything on the plane that is
            //perpendicular to the plane that the host is walking on.
            dir = sensedObject.transform.position - transform.position;
            dir = Vector3.ProjectOnPlane(dir, transform.right);
            dir = Vector3.Normalize(dir);

            forward = Vector3.ProjectOnPlane(transform.forward, transform.right);
            forward = Vector3.Normalize(forward);
            float verticalAngle = Mathf.Rad2Deg * Mathf.Acos(Vector3.Dot(forward, dir));

            //Continue if angle is OK
            if (verticalAngle > fovVertical / 2)
            {
                continue;
            }

            //If the sensor can be blocked by other objects
            if(blockable)
            {
                //Try and send a Raycast from the host to the target. If the hit object
                //is the same as the sensed object, this means that we can see the object.
                RaycastHit hit;
                var rayDirection = sensedObject.transform.position - transform.position;
                if (Physics.Raycast(transform.position, rayDirection, out hit))
                {
                    if (hit.transform != sensedObject.transform)
                    {
                        continue;
                    }
                }
                else
                {
                    continue;
                }
            }
            //Debug.Log("Found " + sensedObject.tag + " with " + sensorType);
            sensedGameObjects.Add(sensedObject);
        }
        return sensedGameObjects.ToArray();
    }

}
