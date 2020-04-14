using System;

public class RangedInt
{

	private int value;
	private int lower;
	private int upper;

	public RangedInt(int value, int lower, int upper)
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

	public RangedInt(int value, int lower)
	: this(value, lower, int.MaxValue)
	{
	}

	public int Add(int amount)
	{
		int newValue = value + amount;
		int oldValue = value;
		if (newValue > upper) // try to add too much
		{
			value = upper;
			return upper - oldValue;
		}
		else if (newValue < lower) // try to subtract too much
		{
			value = lower;
			return lower - oldValue;
		}
		value += amount;
		return amount; // ok
	}

	public int GetValue()
	{
		return value;
	}

	public int GetLower()
	{
		return lower;
	}

	public int GetUpper()
	{
		return upper;
	}

	public RangedInt Duplicate()
	{
		return new RangedInt(value, lower, upper);
	}
}
