using System;

public class GameController
{
    private static double BITE_FACTOR = 0.1;

	public GameController()
	{
	}

    public (double, ConsumptionType) Consume(Animal animal, IConsumable consumable)
    {
        // do eating calculations
        double biteSize = animal.GetSize() * BITE_FACTOR;
        double availableAmount = consumable.GetAmount();
        ConsumptionType type = consumable.GetConsumptionType();
        if (biteSize > consumable.GetAmount()) // trying to consume more than available
        {
            consumable.DecreaseAmount(availableAmount);
            return (availableAmount, type);
        } else // normal case
        {
            consumable.DecreaseAmount(biteSize);
            return (biteSize, type);
        }
    }



}
