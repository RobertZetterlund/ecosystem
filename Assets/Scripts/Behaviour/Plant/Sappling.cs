using System;
using UnityEngine;

public class Sappling : Entity
{

    TickTimer growthTimer;

    public void Init(int size)
    {
        this.size = new RangedDouble(size, 0);
        gameObject.tag = "Sappling";
        //It takes the sappling 30 sec to respawn
        growthTimer = new TickTimer(30);
    }

    void FixedUpdate()
    {
        growthTimer.Tick();
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
