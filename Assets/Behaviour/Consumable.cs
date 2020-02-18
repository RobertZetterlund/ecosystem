using System;

public abstract class Consumable
{
	public Consumable()
	{
	}

    private double maxCapacity;

    double GetMaxCapacity()
    {
        return maxCapacity;
    }

    private double amount; // shouldnt be changed by the rabbit directly, use formula from game controller (i think)

    double GetAmount()
    {
        return amount;
    }
}
