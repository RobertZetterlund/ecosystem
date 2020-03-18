using System;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private int nPlants = 100;
    private static int nRabbits = 50;
    private static int[] nAliveAnimals = new int[Species.GetValues(typeof(Species)).Length];
    private static bool respawn = true;


    void Start()
    {
        // spawn first rabbits
        SpawnAnimal(Species.Rabbit, 1, 1, 3, 0.1, 0.02, 4, new RabbitFCMHandler(FCMFactory.RabbitFCM()), 600);
        // spawn first plants
        for (int i = 0; i < nPlants; i++)
        {
            OrganismFactory.CreatePlant(1, NavMeshUtil.GetRandomLocation());
        }
    }
    

    private static void SpawnAnimal(Species species, double maxSize, double dietFactor, int nChildren, double infantFactor, double growthFactor, double speed, FCMHandler fcmHandler, double heatTimer)
    {
        switch(species)
        {
            case Species.Rabbit:
                for (int i = 0; i < nRabbits; i++)
                {
                    OrganismFactory.CreateAnimal(species, maxSize, dietFactor, nChildren, infantFactor, growthFactor, speed, NavMeshUtil.GetRandomLocation(), fcmHandler, heatTimer);
                }
                break;
            case Species.Plant:

                break;
            default:
                break;
        }
    }

    // register new animal
    public static void Register(Species species)
    {
        nAliveAnimals[(int)species]++;
    }

    // register animal death, spawn new ones if all died
    public static void Unregister(Species species, double maxSize, double dietFactor, int nChildren, double infantFactor, double growthFactor, double speed, FCMHandler fcmHandler, double heatTimer)
    {
        nAliveAnimals[(int)species]--;
        if (respawn)
        {
            if (nAliveAnimals[(int)species] == 0)
            {
                SpawnAnimal(species, maxSize, dietFactor, nChildren, infantFactor, growthFactor, speed, fcmHandler, heatTimer);
            }
        }

    }

}
