using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterPond : MonoBehaviour, IConsumable

{
    public Species specie = Species.Water;


    // Start is called before the first frame update
    void Start()
    {
        gameObject.tag = "Water";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public ConsumptionType GetConsumptionType()
    {
        return ConsumptionType.Water;
    }

    double IConsumable.GetAmount()
    {
        return double.MaxValue;
    }

    double IConsumable.Consume(double amount)
    {
        return -amount; // don't need to do anything if we assume there's infinite amount of water
    }
}
