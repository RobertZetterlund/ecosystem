using System;

public abstract class Consumer
{
	public Consumer(GameController controller)
	{
	}

    GetSize(); // use to calculate "bite size", might just integrate this class into animal superclass

    Consume(Consumable consumable) // maybe dumb idk
    {
        controller.Consume(this, consumable);
    }
}
