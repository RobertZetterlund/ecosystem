﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Plant : Entity, IConsumable
{
    private RangedDouble amountRemaining;
    private int regenTime = 1200; // 20 seconds
    private int regenTimer = 0;
    private UnityEngine.AI.NavMeshAgent navMeshAgent;
    private bool dead = false;

    public void Init(double size)
    {
        this.species = Species.Plant;
        this.size = new RangedDouble(size, 0);
        amountRemaining = new RangedDouble(size*100, 0, size*100);
    }

    // Start is called before the first frame update
    void Start()
    {
        gameObject.tag = "Plant";
        species = Species.Plant;

        //navMeshAgent = gameObject.AddComponent(typeof(UnityEngine.AI.NavMeshAgent)) as UnityEngine.AI.NavMeshAgent;
        //navMeshAgent.speed = 0;
        // calculate properly instead if possible
        //navMeshAgent.baseOffset = OrganismFactory.GetOffset(species);
        //senseRadius = 0;
        //senseRegistrator = new SenseRegistrator(this);
        //sensor = new AreaSensor(transform, senseRegistrator, senseRadius);
    }

    // Update is called once per frame
    void Update()
    {
        regenTimer++;
        if (regenTimer == regenTime)
        {
            amountRemaining.Add(size.GetValue());
        }
    }

    public double Consume(double amount) {

        double consumed = amountRemaining.Add(-amount);

        if (amountRemaining.GetValue() <= 0) {
            //Die(CauseOfDeath.Eaten);
            try
            {
                TransformToSappling();
            } catch (MissingReferenceException)
            {
                Debug.LogWarning("The animals tried to access a consumable that was already destroyed");
            }
            
        }

        return consumed;
    }

    private void TransformToSappling()
    {
        Destroy(gameObject);
        OrganismFactory.CreateSappling((int)size.GetValue(), transform.position);
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
