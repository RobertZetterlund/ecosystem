using System;
using UnityEngine;
using Assets.Scripts;

public class GameController : MonoBehaviour
{
    [SerializeField]
    public bool respawn = true;
    private static bool respawnStatic = true;
    private int nPlants = 40;
    private static int nRabbits = 10;
    private static int[] nAliveAnimals = new int[Species.GetValues(typeof(Species)).Length];
    static TerrainKernal terrainKernal;
    static float[,] heightMap;
    static int sideLength;

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
        terrainKernal = GetComponent<TerrainKernal>();
        heightMap = terrainKernal.GetHeightMap();
        sideLength = terrainKernal.resolution;

        System.Random random = new System.Random();
        
        // spawn first rabbits
        AnimalTraits rabbitTraits = new AnimalTraits(Species.Rabbit, 1, 0, 3, 0.1, 0.02, 3, 600, new RabbitFCMHandler(FCMFactory.RabbitFCM()));
        SpawnAnimal(rabbitTraits);
        // spawn first plants
        int x;
        int z;
        for (int i = 0; i < nPlants; i++)
        {
            //OrganismFactory.CreatePlant(100, NavMeshUtil.GetRandomLocation());
            while(true)
            {
                x = random.Next(1, sideLength);
                z = random.Next(1, sideLength);

                if(heightMap[x, z] < 0.80f && heightMap[x, z] > 0.25f)
                {
                    break;
                }

            }
            
            OrganismFactory.CreatePlant(100, new Vector3(x, terrainKernal.amplifier * terrainKernal.animCurve.Evaluate(heightMap[x, z]), z));

        }
    }

    void Update()
    {
        respawnStatic = respawn;
    }
    private static void SpawnAnimal(AnimalTraits traits)
    {
        System.Random random = new System.Random();
        int x;
        int z;
        switch(traits.species)
        {
            case Species.Rabbit:
                for (int i = 0; i < nRabbits; i++)
                {
                    //Uses an fcmHandler that overrides the GetAction method in the RabbitFCMHandler
                    if(spawnWithManualActions)
                    {
                        traits.fcmHandler = new MockFCMHandler(new RabbitFCMHandler(FCMFactory.RabbitFCM()));
                    }
                    else
                    {
                        traits.fcmHandler = new RabbitFCMHandler(FCMFactory.RabbitFCM());

                        //OrganismFactory.CreateAnimal(traits, NavMeshUtil.GetRandomLocation());
                        while(true) {
                            x = random.Next(1, sideLength);
                            z = random.Next(1, sideLength);

                            if(heightMap[x, z] < 0.80f && heightMap[x, z] > 0.25f) {
                                break;
                            }
                        }
                        OrganismFactory.CreateAnimal(traits, new Vector3(x, terrainKernal.amplifier*terrainKernal.animCurve.Evaluate(heightMap[x, z]), z));
                    }
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
