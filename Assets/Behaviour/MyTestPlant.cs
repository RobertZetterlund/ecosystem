﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MyTestPlant : MonoBehaviour, IConsumable
{
    RangedDouble amountRemaining = new RangedDouble(1, 0, 1);

    // Start is called before the first frame update
    void Start()
    {
        gameObject.tag = "Plant";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public double Consume(double amount) {

        return amountRemaining.Add(-amount);

        if (amountRemaining.GetValue() <= 0) {
            //Die (CauseOfDeath.Eaten);
        }
    }

    public double GetAmount()
    {
        return amountRemaining.GetValue();
    }

    public ConsumptionType GetConsumptionType()
    {
        return ConsumptionType.Plant;
    }
}
