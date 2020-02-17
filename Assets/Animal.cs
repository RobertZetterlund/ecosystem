using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal : MonoBehaviour
{
    double hunger;
    double thirst;
    double timeToDeathByHunger = 200;
    double timeToDeathByThirst = 200;
    bool isDead;
    double energy;

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

        isDead();

    }

    public void isDead(double hunger, double thirst) 
    {
        if (hunger >= 1) 
        {
            Die (CauseOfDeath.Hunger);
        } 
        else if (thirst >= 1) 
        {
            Die (CauseOfDeath.Thirst);
        }
        else if (energy <= 0) 
        {
            Die(CauseOfDeath.Age);
        }
    }

     public booean Die (CauseOfDeath cause)
     {
        if (!isDead) {
            isDead = true;
            //Something.log(cause);
            //Environment.RegisterDeath (this);
            //Destroy (gameObject);
        }
    }
}
