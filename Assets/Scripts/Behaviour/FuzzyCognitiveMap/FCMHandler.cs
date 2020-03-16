using System;
using System.Collections;
using UnityEngine;

/**
 * FCMHandler works as an observer to GameObject so that senses can be passed to the handler
 * from a SenseRegistrator
 * 
 */
public abstract class FCMHandler
{
    protected FCM fcm;

    public FCMHandler(FCM fcm)
    {
        this.fcm = fcm;
    }

    public void CalculateFCM()
    {
        fcm.Calculate();
    }

    public EntityAction GetAction()
    {
        return fcm.GetAction();
    }

    public string GetFCMData()
    {
        return fcm.ToString();
    }

    public abstract void ProcessSensedObjects(Animal animal, ArrayList gameObjects);

    public abstract FCMHandler Reproduce(FCMHandler mateHandler);
}
