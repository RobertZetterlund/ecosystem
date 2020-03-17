using System;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private int nPlants = 100;
    private static int nRabbits = 50;
    private static int[] nAliveAnimals = new int[Species.GetValues(typeof(Species)).Length];


    void Start()
    {
        // spawn first rabbits
        SpawnAnimal(Species.Rabbit, 1, 1, 3, 0.1, 0.02, 4, new RabbitFCMHandler(FCMFactory.RabbitFCM()));
        // spawn first plants
        for (int i = 0; i < nPlants; i++)
        {
            OrganismFactory.CreatePlant(1, NavMeshUtil.GetRandomLocation());
        }

    }

    private static void SpawnAnimal(Species species, double maxSize, double dietFactor, int nChildren, double infantFactor, double growthFactor, double speed, FCMHandler fcmHandler)
    {
        switch(species)
        {
            case Species.Rabbit:
                for (int i = 0; i < nRabbits; i++)
                {
                    OrganismFactory.CreateAnimal(species, maxSize, dietFactor, nChildren, infantFactor, growthFactor, speed, NavMeshUtil.GetRandomLocation(), fcmHandler);
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
    public static void Unregister(Species species, double maxSize, double dietFactor, int nChildren, double infantFactor, double growthFactor, double speed, FCMHandler fcmHandler)
    {
        nAliveAnimals[(int)species]--;
        if (nAliveAnimals[(int)species] == 0)
        {
            SpawnAnimal(species, maxSize, dietFactor, nChildren, infantFactor,growthFactor, speed, fcmHandler);
        }

    }

}
