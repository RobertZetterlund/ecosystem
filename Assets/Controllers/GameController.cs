using System;

public class GameController
{
    private static double BITE_FACTOR = 0.1;

	public GameController()
	{
	}

    public double Consume(Animal animal, IConsumable consumable)
    {
        // do eating calculations
        double biteSize = animal.GetSize() * BITE_FACTOR;
        double availableAmount = consumable.GetAmount();
        if (biteSize > consumable.GetAmount()) // trying to consume more than available
        {
            consumable.DecreaseAmount(availableAmount);
            return availableAmount;
        } else // normal case
        {
            consumable.DecreaseAmount(biteSize);
            return biteSize;
        }
    }



}
