using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterPond : MonoBehaviour

{
    double amount = 1;
    double capacity = 1;

    double consume(double amount)
    {
        if (this.amount - amount < 0) // wants to drink more than what is available
        {
            double consumed = this.amount;
            this.amount = 0;
            return consumed;
        } else // can consome the desired amount
        {
            this.amount -= amount;
            return amount;
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
