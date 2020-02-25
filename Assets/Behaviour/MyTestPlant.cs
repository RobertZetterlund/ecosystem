using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MyTestPlant : MonoBehaviour
{

    RangedDouble amountRemaining = new RangedDouble(1, 0, 1);


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public double Consume(double amount) {

        return amountRemaining.add(-amount);

        if (amountRemaining.value <= 0) {
            //Die (CauseOfDeath.Eaten);
        }
    }

    public double GetAmount()
    {
        return amountRemaining.value;
    }

    public ConsumptionType GetConsumptionType()
    {
        return ConsumptionType.Plant;
    }
}
