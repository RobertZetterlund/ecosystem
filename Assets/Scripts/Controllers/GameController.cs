using System;
using UnityEngine;
using Assets.Scripts;

public class GameController : MonoBehaviour
{
    [SerializeField]
    public bool respawn = true;
    private static bool respawnStatic = true;
    private int nPlants = 5;
    private static int nRabbits = 10;
    private static int[] nAliveAnimals = new int[Species.GetValues(typeof(Species)).Length];

    [Range(1f,100)]
    public float gameSpeed = 1;

    //Spawns all animals with the ability of being controlled in the editor. (Can still be done individually)
    //The reason that there are two variables is because you can't set a static variable through the editor sadly.
    public static bool spawnWithManualActions = false;
    public bool _spawnWithManualActions = false;

    public static bool animalCanDie = true;
    public bool _animalCanDie = true;


    void Start()
    {
        // spawn first rabbits
        AnimalTraits rabbitTraits = new AnimalTraits(Species.Rabbit, 3, 0, 2, 0.01, 0.002, 3, 600, new RabbitFCMHandler(FCMFactory.RabbitFCM()));
        SpawnAnimal(rabbitTraits);
        // spawn first plants
        for (int i = 0; i < nPlants; i++)
        {
            OrganismFactory.CreatePlant(100, NavMeshUtil.GetRandomLocation());
        }
    }

    void Update()
    {
        respawnStatic = respawn;
    }
    private static void SpawnAnimal(AnimalTraits traits)
    {
        switch(traits.species)
        {
            case Species.Rabbit:
                for (int i = 0; i < nRabbits; i++)
                {
                    AnimalTraits traitsCopy = traits.Duplicate();
                    //Uses an fcmHandler that overrides the GetAction method in the RabbitFCMHandler
                    if(spawnWithManualActions) 
                        traitsCopy.fcmHandler = new MockFCMHandler(new RabbitFCMHandler(FCMFactory.RabbitFCM()));
                    else
                        traitsCopy.fcmHandler = new RabbitFCMHandler(FCMFactory.RabbitFCM());

                    OrganismFactory.CreateAnimal(traitsCopy, NavMeshUtil.GetRandomLocation());
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
        if (respawnStatic)
        {
            if (nAliveAnimals[(int)traits.species] == 0)
            {
                SpawnAnimal(traits);
            }
        }

    }

    private void OnValidate()
    {
        Time.timeScale = gameSpeed;
        spawnWithManualActions = _spawnWithManualActions;
        animalCanDie = _animalCanDie;
    }


}
