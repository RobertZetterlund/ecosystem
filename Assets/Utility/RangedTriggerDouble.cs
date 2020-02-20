using System;

public class RangedTriggerDouble
{

    private double value;
    private double lower;
    private double upper;

	public RangedTriggerDouble(double value, double lower, double upper)
	{
        this.value = value;
        this.lower = lower;
        this.upper = upper;
	}

    public RangedTriggerDouble(double value, double lower)
    {
        this.value = value;
        this.lower = lower;
        upper = double.MaxValue;
    }

    public double add(double amount)
    {
        double newValue = value + amount;
        double oldValue = value;
        if (newValue > upper) // try to add too much
        {
            value = upper;
            return upper - oldValue;
        } else if(newValue < lower) // try to subtract too much
        {
            value = lower;
            return lower - oldValue;
        }
        value += amount;
        return amount; // ok
    }

    public double getValue()
    {
        return value;
    }
}
