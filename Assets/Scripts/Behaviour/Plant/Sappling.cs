using System;
using UnityEngine;

public class Sappling : MonoBehaviour
{

    Timer growthTimer;
    private int size;

    public void Init(int size)
    {
        this.size = size;
        gameObject.tag = "Sappling";
        //It takes the sappling 30 sec to respawn
        growthTimer = new Timer(30);
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
        OrganismFactory.CreatePlant(size, transform.position);
    }
}
