using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GoToConsumable : AbstractAction
{
    private SearchAction search;
    private TickTimer eatTimer;
    private ConsumptionType consumable;

    public GoToConsumable(Animal animal, ConsumptionType consumable) : base(animal)
    {
        //Slighlty different mechanics for finding water
        if (consumable == ConsumptionType.Water)
            search = new SearchWaterAction(animal);
        else
            search = new SearchAction(animal);
        this.consumable = consumable;
        eatTimer = new TickTimer(1);
    }

    private void Search()
    {
        search.Execute();
        if (search.IsDone())
            Eat();
    }

    private void Eat()
    {
        state = ActionState.Eating;
        IConsumable consumable = null;
        if(animal.targetGameObject == null)
        {
            //Found food does not exist, GO back to search phase
            Reset();
            return;
        }

        switch (this.consumable)
        {
            case ConsumptionType.Animal:
                consumable = animal.targetGameObject.GetComponent<Animal>();
                break;
            case ConsumptionType.Plant:
                consumable = animal.targetGameObject.GetComponent<Plant>();
                break;
            case ConsumptionType.Water:
                consumable = animal.targetGameObject.GetComponent<Water>();
                break;
        }
        eatTimer.Tick();
        if(eatTimer.IsDone())
        {
            animal.Eat(consumable);
            eatTimer.Reset();
            if (consumable == null || consumable.GetAmount() <= 0)
            {
                //Debug.Log("Bites: " + bites + "  Plants: " + plants);
                Reset();
            }   
        }
        
        
    }
    public override void Execute()
    {
        switch (state)
        {
            case ActionState.Eating:
                Eat();
                break;
            default:
                Search();
                break;
        }
    }

    public override void Reset()
    {
        state = 0;
    }

    public override bool IsDone()
    {
        throw new NotImplementedException();
    }
}

