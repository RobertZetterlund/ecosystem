using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SenseRegistrator
{
    protected FCM fcm;

    public SenseRegistrator(FCM fcm)
    {
        this.fcm = fcm;
    }
    public abstract void Register(GameObject sensedObject);


    //Register senses by using multiple colliders at once
    public void Register(Collider[] colliders)
    {
        foreach (Collider c in colliders)
        {
            GameObject sensedGameObject = c.gameObject;
            Register(sensedGameObject);
        }
    }
}
