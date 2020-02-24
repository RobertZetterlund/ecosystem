using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MyTestPlant : MonoBehaviour
{
    double amountRemaining = 1;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.tag = "Plant";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Consume(double amount) {
        amountRemaining -= amount;

        if (amountRemaining <= 0) {
            //Die (CauseOfDeath.Eaten);
        }
    }

    public double GetAmount()
    {
        return amountRemaining;
    }

    public ConsumptionType GetConsumptionType()
    {
        return ConsumptionType.Plant;
    }
}
