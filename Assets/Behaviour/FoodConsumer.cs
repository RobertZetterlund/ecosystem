using System;

public abstract class FoodConsumer
{

    GameController controller;

    public FoodConsumer(GameController controller)
	{
        this.controller = controller;
	}

    //foodConsumer(); // use to calculate "bite size", might just integrate this class into animal superclass

    public void consumeFood(Consumable consumable) // maybe dumb idk
    {
        controller.Consume(this, consumable);
    }
}
