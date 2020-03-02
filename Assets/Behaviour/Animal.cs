﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class Animal : MonoBehaviour, IConsumable
{
    RangedDouble hunger = new RangedDouble(0, 0, 1);
    RangedDouble thirst = new RangedDouble(0, 0, 1);
    double timeToDeathByHunger = 200;
    double timeToDeathByThirst = 200;
    private static double BITE_FACTOR = 0.2; // use to calculate how much you eat in one bite
    double lifespan = 2000;
    bool dead;
    double energy = 1;
    RangedDouble health = new RangedDouble(1, 0, 1); //max health should be 1, health scaling depends on size
    GameController controller;
    RangedDouble size;
    RangedDouble dietFactor; // 1 = carnivore, 0.5 = omnivore, 0 = herbivore
    protected EntityAction currentAction = EntityAction.Idle;
	private NavMeshAgent navMeshAgent;
    private FCM fcm;
    private SenseRegistrator senseRegistrator;
    private float senseRadius;
    private ISensor sensor;
    private float lastFCMUpdate = 0;
    private bool isMale;
    private AnimalType type;

    //Debugging
    Color SphereGizmoColor = new Color(1, 1, 0, 0.3f);
    public bool showFCMGizmo, showSenseRadiusGizmo = false;

    public void Init(GameController controller, double size, double dietFactor)
    {
        this.controller = controller;
        this.dietFactor = new RangedDouble(dietFactor, 0, 1);
        this.size = new RangedDouble(size, 0);

        System.Random rand = new System.Random();
        isMale = rand.NextDouble() >= 0.5;
    }

    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = gameObject.AddComponent(typeof(NavMeshAgent)) as NavMeshAgent;
        navMeshAgent.speed = 5;
        senseRadius = 15;
        fcm = FCMFactory.RabbitFCM();
        senseRegistrator = new SenseRegistrator(this);
        sensor = new AreaSensor(transform, senseRegistrator, senseRadius);

    }

    // TODO remove, hard coded reproduction
    int counter = 0;
    // Update is called once per frame
    void Update()
    {
        // TODO remove, hard coded reproduction
        counter++;
        if (counter == 300)
        {
            Reproduce(this);
        }
        //increases hunger and thirst over time
        hunger.Add(Time.deltaTime * 1 / timeToDeathByHunger);
        thirst.Add(Time.deltaTime * 1 / timeToDeathByThirst);

        //age the animal
        energy -= Time.deltaTime * 1/lifespan;


        sensor.Sense();
        if((Time.time - lastFCMUpdate) > 1)
        {
            lastFCMUpdate = Time.time;
            fcm.Calculate();
        }
        
        chooseNextAction();

        //check if the animal is dead
        isDead();

    }



    public void isDead() 
    {
        if (hunger.GetValue() >= 1) 
        {
            Die(CauseOfDeath.Hunger);
        } 
        else if (thirst.GetValue() >= 1) 
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
        if (hunger.GetValue() >= thirst.GetValue() || (eating && !isCriticallyThirsty()))
        {
            findFood();
        }
        // More thirsty than hungry
        else if (thirst.GetValue() > hunger.GetValue() || (drinking && !isCriticallyHungry()))
        {
            findWater();
        }
        


        //doAction();
            // a method that makes the animal eat, drink, or reproduce
    }

    private void findFood()
    {
        //some shit here
        //currentAction = EntityAction.GoingToFood;
    }

    private void findWater()
    {
        //some shit here
        //currentAction = EntityAction.GoingToWater;
    }

    private void findMate()
    {
        //Some shit here
        //currentAction = EntityAction.SearchingForMate;
    }

    public bool isCriticallyThirsty()
    {
        return thirst.GetValue() < 0.1; //change these values when we know more or avoid hardcoded values
    }

    public bool isCriticallyHungry()
    {
        return hunger.GetValue() < 0.1; //change these values when we know more or avoid hardcoded values
    }
    public void Reproduce(Animal mate)
    {
        if (hunger.GetValue() < 0.3 && thirst.GetValue() < 0.6)
        {
            if (energy > 0.4)
            {
                //code here for reproduction
                controller.Reproduce(this, mate);
            }
            //code here for sex
            currentAction = EntityAction.Idle; // Set action to idle when done
        }
    }

    // let this animal attempt to take a bite from the given consumable
    private void Consume(IConsumable consumable)
    {
        // do eating calculations
        double biteSize = size.GetValue() * BITE_FACTOR;
        ConsumptionType type = consumable.GetConsumptionType();

        swallow(consumable.Consume(biteSize), type);
    }



    public double GetAmount()
    {
        return size.GetValue() * health.GetValue(); 
    }

    public RangedDouble GetSize()
    {
        return size.Duplicate();
    }

    public RangedDouble GetDiet()
    {
        return dietFactor.Duplicate();
    }

    // eat this animal
    public double Consume(double amount)
    {
        return health.Add(-amount/size.GetValue());
    }

    public ConsumptionType GetConsumptionType()
    {
        return ConsumptionType.Animal;
    }

    // swallow the food/water that this animal ate
    private void swallow(double amount, ConsumptionType type)
    {
        amount /= size.GetValue(); // balance according to size. (note that amount will be higher if youre size is bigger)
        // increment energy / hunger / thirst
        switch (type)
        {
            case ConsumptionType.Water:
                thirst.Add(-amount);
                break;
            case ConsumptionType.Animal:
                hunger.Add(-amount*dietFactor.GetValue());
                break;
            case ConsumptionType.Plant:
                hunger.Add(-amount*(1-dietFactor.GetValue()));
                break;
        }
    }
	
	public void SetDestination(Vector3 destination)
    {
        navMeshAgent.SetDestination(destination);
    }

    public FCM GetFCM()
    {
        return fcm;
    }

    public float GetSenseRadius()
    {
        return senseRadius;
    }

    //Draws a sphere corresponding to its sense radius
    void OnDrawGizmos()
    {
        if(showFCMGizmo)
        {
            Vector3 textOffset = new Vector3(-3, 2, 0);
            Handles.Label(transform.position + textOffset, currentAction.ToString());
            textOffset = new Vector3(1, 2, 0);
            if (fcm != null)
                Handles.Label(transform.position + textOffset, fcm.ToString());
        }

        if(showSenseRadiusGizmo)
        {
            Gizmos.color = SphereGizmoColor;
            Gizmos.DrawSphere(transform.position, senseRadius);
        }
        
        
    }

    public bool GetIsMale()
    {
        return isMale;
    }

    public void SetType(AnimalType type)
    {
        this.type = type;
    }

    public AnimalType GetType()
    {
        return type;
    }
}