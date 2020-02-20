using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal : MonoBehaviour, IMovement
{
    double hunger;
    double thirst;
    double timeToDeathByHunger = 200;
    double timeToDeathByThirst = 200;
    double lifespan = 2000;
    bool dead;
    double energy;
    public float speed = 5f;
    GameController controller;
    //Movement implemented through strategy pattern
    private IMovement movement;

    public Animal(GameController controller)
    {
        this.controller = controller;
    }

    // Start is called before the first frame update
    public virtual void Start()
    {
        //TransformMovement implemented for now, might be better to switch to RBMovement further on
        movement = new TransformMovement(transform);
        SetSpeed(speed);
        // use Köres senses to do tings. innit bruv
    }

    // Update is called once per frame
    public virtual void Update()
    {
        //increases hunger and thirst over time
        hunger += Time.deltaTime * 1 / timeToDeathByHunger;
        thirst += Time.deltaTime * 1 / timeToDeathByThirst;

        //age the animal
        energy -= Time.deltaTime * 1/lifespan;


        //check if the animal is dead
        isDead();

        //move the animal
        Move();

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

    public void consumeFood(Consumable consumable)
    {
        controller.Consume(this, consumable);
    }


    public void Move()
    {
        movement.Move();
    }

    public void Stop()
    {
        movement.Stop();
    }

    public Vector3 GetDirection()
    {
        return movement.GetDirection();
    }

    public void SetSpeed(float speed)
    {
        movement.SetSpeed(speed);
    }

    public float GetSpeed()
    {
        return movement.GetSpeed();
    }

    public void SetTargetDestination(Vector3 destination)
    {
        movement.SetTargetDestination(destination);
    }

    public bool HasReachedDestination()
    {
        return movement.HasReachedDestination();
    }
}
