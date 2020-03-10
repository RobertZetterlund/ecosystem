using System;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private int nPlants = 10;
    private int nRabbits = 5;

    public void SpawnEntities()
    {
        // spawn first rabbits
        for (int i = 0; i < nRabbits; i++)
        {
            OrganismFactory.CreateAnimal(Species.Rabbit, 1, 1, 3, 0.1, 0.02, 4, NavMeshUtil.GetRandomLocation());
        }
        // spawn first plants
        for (int i = 0; i < nPlants; i++)
        {
            OrganismFactory.CreatePlant(1, NavMeshUtil.GetRandomLocation());
        }
    }

    public void SpawnRabbit()
    {
        OrganismFactory.CreateAnimal(Species.Rabbit, 1, 1, 3, 0.1, 0.02, 4, new Vector3(0,0,0));
    }

    public void SpawnPlant()
    {
        OrganismFactory.CreatePlant(1, new Vector3(0, 0, 10));
    }



}
