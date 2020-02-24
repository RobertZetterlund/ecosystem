using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;

public abstract class Animal : MonoBehaviour, IConsumable
{
    double hunger;
    double thirst;
    double timeToDeathByHunger = 200;
    double timeToDeathByThirst = 200;
    private static double BITE_FACTOR = 0.2; // use to calculate how much you eat in one bite
    double lifespan = 2000;
    bool dead;
    double energy;
    double health; //max health should be 1, health scaling depends on size
    GameController controller;
    EntityAction currentAction;
    double size;
    double dietFactor; // 1 = carnivore, 0.5 = omnivore, 0 = herbivore
	private NavMeshAgent navMeshAgent;
    private FCM fcm;

    public Animal(GameController controller)
    {
        this.controller = controller;
    }

    // Start is called before the first frame update
    public virtual void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = 5;
    }

    // Update is called once per frame
    public virtual void Update()
    {
        //increases hunger and thirst over time
        hunger += Time.deltaTime * 1 / timeToDeathByHunger;
        thirst += Time.deltaTime * 1 / timeToDeathByThirst;

        //age the animal
        energy -= Time.deltaTime * 1/lifespan;

        chooseNextAction();

        //check if the animal is dead
        isDead();

    }


    public void isDead() 
    {
        if (hunger >= 1) 
        {
            Die(CauseOfDeath.Hunger);
        } 
        else if (thirst >= 1) 
        {
            Die(CauseOfDeath.Thirst);
        }
        else if (energy <= 0) 
        {
            Die(CauseOfDeath.Age);
        }
    }

     public void Die(CauseOfDeath cause)
     {
        if (!dead) {
            dead = true;
            //Something.log(cause);
            //Environment.RegisterDeath (this);
            //Destroy (gameObject);
        }
        
    }

    public void chooseNextAction()
    {


        currentAction = fcm.GetAction();
            //Köre har något här hoppas jag
        if (EntityAction.Idle == currentAction || EntityAction.Resting == currentAction) // && Maybe mate nearby or maybe theyre always searching
        {
            findMate();
        }

        // Get current action
        bool eating = currentAction == EntityAction.Eating;
        bool drinking = currentAction == EntityAction.Drinking;
        //bool reproducing = currentAction == EntityAction.Reproducing; //Dont think we should allow animals to stop having sex. Once started they will finish


        // More hungry than thirsty
        if (hunger >= thirst || (eating && !isCriticallyThirsty()))
        {
            findFood();
        }
        // More thirsty than hungry
        else if (thirst > hunger || (drinking && !isCriticallyHungry()))
        {
            findWater();
        }
        


        //doAction();
            // a method that makes the animal eat, drink, or reproduce
    }

    private void findFood()
    {
        //some shit here
        currentAction = EntityAction.GoingToFood;
    }

    private void findWater()
    {
        //some shit here
        currentAction = EntityAction.GoingToWater;
    }

    private void findMate()
    {
        //Some shit here
        currentAction = EntityAction.SearchingForMate;
    }

    public bool isCriticallyThirsty()
    {
        return thirst < 0.1; //change these values when we know more or avoid hardcoded values
    }

    public bool isCriticallyHungry()
    {
        return hunger < 0.1; //change these values when we know more or avoid hardcoded values
    }
    public void reproduce()
    {
        if (hunger < 0.3 && thirst < 0.6)
        {
            if (energy > 0.4)
            {
                //code here for reproduction
            }
            //code here for sex
            currentAction = EntityAction.Idle; // Set action to idle when done
        }
    }

    // let this animal attempt to take a bite from the given consumable
    private void Consume(IConsumable consumable)
    {
        // do eating calculations
        double biteSize = size * BITE_FACTOR;
        double availableAmount = consumable.GetAmount();
        ConsumptionType type = consumable.GetConsumptionType();

        double amountConsumed = Math.Min(availableAmount, biteSize);
        consumable.Consume(amountConsumed);
        swallow(amountConsumed, type);

    }



    public double GetAmount()
    {
        return size * health; 
    }

    public double GetSize()
    {
        return size;
    }

    // eat this animal
    public void Consume(double amount)
    {
        health -= amount / size;
    }

    public ConsumptionType GetConsumptionType()
    {
        return ConsumptionType.Animal;
    }

    // swallow the food/water that this animal ate
    private void swallow(double amount, ConsumptionType type)
    {
        amount /= size; // balance according to size. (note that amount will be higher if youre size is bigger)
        // increment energy / hunger / thirst
        switch (type)
        {
            case ConsumptionType.Water:
                thirst -= amount;
                break;
            case ConsumptionType.Animal:
                hunger -= amount * dietFactor;
                break;
            case ConsumptionType.Plant:
                hunger -= amount * (1 - dietFactor);
                break;
        }
    }
	
	public void SetDestination(Vector3 destination)
    {
        navMeshAgent.SetDestination(destination);
    }

    


}
