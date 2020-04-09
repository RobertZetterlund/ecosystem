using System;
using UnityEngine;

public class Sappling : Entity
{

    Timer growthTimer;

    public void Init(int size)
    {
        this.size = new RangedDouble(size, 0);
        gameObject.tag = "Sappling";
        //It takes the sappling 30 sec to respawn
        growthTimer = new Timer(60);
        growthTimer.Reset();
        growthTimer.Start();
    }

    private void Update()
    {
        if (growthTimer.IsDone())
        {
            TransformToPlant();
        }
    }

    private void TransformToPlant()
    {
        Destroy(gameObject);
        OrganismFactory.CreatePlant((int)size.GetValue(), transform.position);
    }
}
