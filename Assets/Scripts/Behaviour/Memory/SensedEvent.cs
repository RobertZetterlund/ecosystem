﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class SensedEvent
{
    private GameObject Water;
    private GameObject Foe;
    private GameObject Mate;
    private GameObject Food;
    private IDictionary<string,int> weightMap;

    public SensedEvent(IDictionary<string,int> weightMap, GameObject Water, GameObject Foe, GameObject Mate, GameObject Food)
    {
        this.weightMap = weightMap;
        this.Water= Water;
        this.Foe= Foe;
        this.Mate= Mate;
        this.Food= Food;
    }

    public IDictionary<string,int> GetWeightMap()
    {
        return weightMap;
    }

    public GameObject GetWater()
    {
        return Water;
    }
    public GameObject GetFood()
    {
        return Food;
    }
    public GameObject GetFoe()
    {
        return Foe;
    }
    public GameObject GetMate()
    {
        return Mate;
    }


}