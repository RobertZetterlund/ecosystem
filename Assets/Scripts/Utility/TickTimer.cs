using System;

public class TickTimer
{
    private float std_fixed_update = 0.25f;
    private float interval;
    private float ticks = 0;
    private int tick_interval;

    public TickTimer(float interval)
    {
        this.interval = interval;
        tick_interval = (int)Math.Round(interval / std_fixed_update, 0);
        //ticks = (int)MathUtility.RandomUniform(0, tick_interval - 1);
    }

    public void Tick()
    {
        ticks++;
    }

    public bool IsDone()
    {
        return ticks >= tick_interval;
    }

    public void Reset()
    {
        ticks = 0;
    }


}

