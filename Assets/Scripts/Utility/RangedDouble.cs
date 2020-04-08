using System;

public class RangedDouble
{

    private double value;
    private double lower;
    private double upper;

	public RangedDouble(double value, double lower, double upper)
	{
        if (upper < lower)
        {
            throw new System.ArgumentException("Bounds overlap");
        }

        if (value > upper || value < lower)
        {
            throw new System.ArgumentException("value outside bounds");
        }

        this.value = value;
        this.lower = lower;
        this.upper = upper;
	}

    public RangedDouble(double value, double lower)
    : this(value, lower, double.MaxValue)
    { 
    }

    public double Add(double amount)
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

    public void SetValue(double value)
    {
        this.value = value;
    }

    public double GetValue()
    {
        return value;
    }

    public double GetLower()
    {
        return lower;
    }

    public double GetUpper()
    {
        return upper;
    }

    public RangedDouble Duplicate()
    {
        return new RangedDouble(value, lower, upper);
    }
}
