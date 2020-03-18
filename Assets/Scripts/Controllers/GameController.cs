using System;
using UnityEngine;
using Assets.Scripts;

public class GameController : MonoBehaviour
{
    private int nPlants = 100;
    private static int nRabbits = 3;
    private static int[] nAliveAnimals = new int[Species.GetValues(typeof(Species)).Length];
    private static bool respawn = true;


    void Start()
    {
        // spawn first rabbits
        AnimalTraits traits = new AnimalTraits(Species.Rabbit, 1, 1, 3, 0.1, 0.02, 4, 600, new RabbitFCMHandler(FCMFactory.RabbitFCM()));
        SpawnAnimal(traits);
        // spawn first plants
        for (int i = 0; i < nPlants; i++)
        {
            OrganismFactory.CreatePlant(1, NavMeshUtil.GetRandomLocation());
        }
    }
    

    private static void SpawnAnimal(AnimalTraits traits)
    {
        switch(traits.species)
        {
            case Species.Rabbit:
                for (int i = 0; i < nRabbits; i++)
                {
                    OrganismFactory.CreateAnimal(traits, NavMeshUtil.GetRandomLocation());
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
    public static void Unregister(AnimalTraits traits)
    {
        nAliveAnimals[(int)traits.species]--;
        if (respawn)
        {
            if (nAliveAnimals[(int)traits.species] == 0)
            {
                SpawnAnimal(traits);
            }
        }

    }

}
