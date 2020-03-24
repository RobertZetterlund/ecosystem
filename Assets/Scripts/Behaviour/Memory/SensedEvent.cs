using System;
using System.Collections.Generic;
using UnityEngine;

public class SensedEvent
{
    private GameObject Water;
    private GameObject Foe;
    private GameObject Mate;
    private GameObject Food;

    public SensedEvent(GameObject Water, GameObject Foe, GameObject Mate, GameObject Food)
    {
        this.Water= Water;
        this.Foe= Foe;
        this.Mate= Mate;
        this.Food= Food;
    }


    public GameObject getWater()
    {
        return Water;
    }
    public GameObject getFood()
    {
        return Food;
    }
    public GameObject getFoe()
    {
        return Foe;
    }
    public GameObject getMate()
    {
        return Mate;
    }


}