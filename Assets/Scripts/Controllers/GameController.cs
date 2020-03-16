using System;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private int nPlants = 100;
    private int nRabbits = 50;


    void Start()
    {
        // spawn first rabbits
        for (int i = 0; i < nRabbits; i++)
        {
            OrganismFactory.CreateAnimal(Species.Rabbit, 1, 1, 3, 0.1, 0.02, 4, NavMeshUtil.GetRandomLocation(), new RabbitFCMHandler(FCMFactory.RabbitFCM()));
        }
        // spawn first plants
        for (int i = 0; i < nPlants; i++)
        {
            OrganismFactory.CreatePlant(1, NavMeshUtil.GetRandomLocation());
        }



    }



}
