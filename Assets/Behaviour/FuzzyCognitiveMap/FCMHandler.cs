using System;
using UnityEngine;

/**
 * FCMHandler works as an observer to GameObject so that senses can be passed to the handler
 * from a SenseRegistrator
 * 
 */
abstract class FCMHandler : IObserver<GameObject>
{
    protected FCM fcm;
    protected Animal animal;

    public FCMHandler(Animal animal)
    {
        this.animal = animal;
    }

    public virtual void CalculateFCM()
    {
        fcm.Calculate();
    }

    public virtual EntityAction GetAction()
    {
        return fcm.GetAction();
    }

    public void OnCompleted()
    {
        throw new NotImplementedException();
    }

    public void OnError(Exception error)
    {
        throw new NotImplementedException();
    }

    public abstract void OnNext(GameObject value);

    public string GetFCMData()
    {
        return fcm.ToString();
    }

}
