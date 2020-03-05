using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * This is currently the "Hub" for registrating senses. I'm imagining that all Sensors will broadcast to here,
 * and then this SenseRegistrator will carry the message forward to whoever needs it.
 * 
 * Currently, when it registers a sense it just sends it to all observers that are listening.
 * 
 * I think this can be reworked to be smarter but I don't know how atm.
 * 
 * 
 * 
 * 
 */
public class SenseRegistrator : IObservable<GameObject>
{

    private List<IObserver<GameObject>> observers;

    public SenseRegistrator()
    {
        observers = new List<IObserver<GameObject>>();
    }

    // broadcast to all observers if a sensedObject is found
    public void Register(GameObject sensedObject)
    {
        foreach (var observer in observers)
        {
            observer.OnNext(sensedObject);
        }
    }

    // Subscribe to the SenseRegistrator via observer pattern
    public IDisposable Subscribe(IObserver<GameObject> observer)
    {
        if (!observers.Contains(observer))
            observers.Add(observer);
        return new Unsubscriber(observers, observer);
    }

    // This is observer pattern stuff. Basically makes it easy to unsubscribe as far as I understand it
    private class Unsubscriber : IDisposable
    {
        private List<IObserver<GameObject>> _observers;
        private IObserver<GameObject> _observer;

        public Unsubscriber(List<IObserver<GameObject>> observers, IObserver<GameObject> observer)
        {
            this._observers = observers;
            this._observer = observer;
        }

        public void Dispose()
        {
            if (_observer != null && _observers.Contains(_observer))
                _observers.Remove(_observer);
        }
    }
}
