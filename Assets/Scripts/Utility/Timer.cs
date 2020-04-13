using System;
using UnityEngine;

public class Timer
{
    float interval;
    float startTime;
    bool done = false;
    bool paused = true;

    public Timer(float interval)
    {
        this.interval = interval;
        Reset();
    }

    public void Reset()
    {
        startTime = Time.time;
        done = false;
    }

    public void Start()
    {
        paused = false;
    }

    public void Pause()
    {
        done = IsDone();
        paused = true;
    }

    public bool IsDone()
    {
        if(paused)
        {
            return done;
        }
        return TimeSinceStart() > interval;
    }

    public void SetInterval(float interval)
    {
        this.interval = interval;
    }

    public float TimeSinceStart()
    {
        return (Time.time - startTime) * SimulationController.Instance().gameSpeed;
    }

}

