using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal : MonoBehaviour, IConsumable
{
    double hunger;
    double thirst;
    double timeToDeathByHunger = 200;
    double timeToDeathByThirst = 200;
    double lifespan = 2000;
    bool dead;
    double energy;
    double health;
    double maxHealth;
    GameController controller;
    double size;

    public Animal(GameController controller)
    {
        this.controller = controller;
    }

    // Start is called before the first frame update
    void Start()
    {
        // use Köres senses to do tings. innit bruv
    }

    // Update is called once per frame
    void Update()
    {
        //increases hunger and thirst over time
        hunger += Time.deltaTime * 1 / timeToDeathByHunger;
        thirst += Time.deltaTime * 1 / timeToDeathByThirst;

        //age the animal
        energy -= Time.deltaTime * 1/lifespan;


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

    public void reproduce() 
    {
        if(hunger < 0.3 && thirst < 0.6) {
            if(energy > 0.4)
            {
                //code here for reproduction
            }
            //code here for sex
        }
    }

    public void consumeFood(IConsumable consumable)
    {
        controller.Consume(this, consumable);
    }

    public double GetAmount()
    {
        return size * (health / maxHealth); 
    }

    public double GetSize()
    {
        return size;
    }

    public void DecreaseAmount(double amount)
    {
        health -= amount * maxHealth / size;
    }

    public ConsumptionType GetConsumptionType()
    {
        return ConsumptionType.Animal;
    }


}
