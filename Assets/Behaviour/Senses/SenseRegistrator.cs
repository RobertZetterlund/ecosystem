using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SenseRegistrator : IObservable<GameObject>
{

    private List<IObserver<GameObject>> observers;

    public SenseRegistrator()
    {
        observers = new List<IObserver<GameObject>>();
    }

    public void Register(GameObject sensedObject)
    {
        foreach (var observer in observers)
        {
            observer.OnNext(sensedObject);
        }
    }

    public IDisposable Subscribe(IObserver<GameObject> observer)
    {
        if (!observers.Contains(observer))
            observers.Add(observer);
        return new Unsubscriber(observers, observer);
    }

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
