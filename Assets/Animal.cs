using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal : MonoBehaviour
{
    double hunger;
    double thirst;
    double timeToDeathByHunger = 200;
    double timeToDeathByThirst = 200;
    double lifespan = 2000;
    bool dead;
    double energy;
    private Rigidbody rb;
    public float speed = 5f;
    private Vector3 direction = new Vector3(0, 0, 0);
    GameController controller;

    public Animal(GameController controller)
    {
        this.controller = controller;
    }

    // Start is called before the first frame update
    public void Start()
    {
        Debug.Log("Yes");
        rb = GetComponent<Rigidbody>();
        // use Köres senses to do tings. innit bruv
    }

    // Update is called once per frame
    public void Update()
    {
        //increases hunger and thirst over time
        hunger += Time.deltaTime * 1 / timeToDeathByHunger;
        thirst += Time.deltaTime * 1 / timeToDeathByThirst;

        //age the animal
        energy -= Time.deltaTime * 1/lifespan;


        //check if the animal is dead
        isDead();

        //move the animal
        Move(direction);

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

    public void Move(Vector3 location)
    {
        // The step size is equal to speed times frame time.
        float singleStep = 8 * Time.deltaTime;

        // Rotate the forward vector towards the target direction by one step
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, location, singleStep, 0.0f);

        // Draw a ray pointing at our target in
        Debug.DrawRay(transform.position, newDirection, Color.red);

        // Calculate a rotation a step closer to the target and applies rotation to this object
        rb.MoveRotation(Quaternion.LookRotation(newDirection));
        rb.MovePosition(transform.position + direction * speed * Time.deltaTime);
    }

    public void SetDirection(Vector3 direction)
    {
        this.direction = direction;
    }
}
