using System;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private int nPlants = 10;
    private int nRabbits = 5;


    void Start()
    {
        // spawn first rabbits
        for (int i = 0; i < nRabbits; i++)
        {
            OrganismFactory.CreateAnimal(Species.Rabbit, 1, 1, 3, NavMeshUtil.GetRandomLocation());
        }
        // spawn first plants
        for (int i = 0; i < nPlants; i++)
        {
            OrganismFactory.CreatePlant(1,NavMeshUtil.GetRandomLocation());
        }



    }



}
