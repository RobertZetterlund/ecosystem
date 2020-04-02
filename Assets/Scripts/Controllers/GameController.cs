using System;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField]
    public bool respawn = true;
    private static bool respawnStatic = true;
    private int nPlants = 400;
    private static int[] nAnimals = new int[Species.GetValues(typeof(Species)).Length];
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
    public int nrOfRabbits;
    public int nrOfFoxes;


    void Start()
    {

        nAnimals[(int)Species.Rabbit] = nrOfRabbits;
        nAnimals[(int)Species.Fox] = nrOfFoxes;


        GameObject gameMaster = GameObject.Find("Game Master");
        terrainKernal = gameMaster.GetComponent<TerrainKernal>();
        heightMap = terrainKernal.GetHeightMap();
        sideLength = terrainKernal.resolution;
        

        System.Random random = new System.Random();

        // spawn first foxes
        AnimalTraits foxTraits = new AnimalTraits(Species.Fox, 3, 0, 2, 0.1, 0.2, 3, 20, 30, 25, new FoxFCMHandler(FCMFactory.FoxFCM())); //ght need a foxFCM later on
        SpawnAnimal(foxTraits);
        
        // spawn first rabbits
        AnimalTraits rabbitTraits = new AnimalTraits(Species.Rabbit, 3, 0, 2, 0.1, 0.2, 3, 20, 30, 25, new RabbitFCMHandler(FCMFactory.RabbitFCM()));
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
        for (int i = 0; i < nAnimals[(int)traits.species]; i++)
        {
            AnimalTraits trait = traits.Duplicate();

            //Uses an fcmHandler that overrides the GetAction method in the RabbitFCMHandler
            int x;
            int z;
            //OrganismFactory.CreateAnimal(trait, NavMeshUtil.GetRandomLocation());
            while (true)
            {

                x = random.Next(1, sideLength);
                z = random.Next(1, sideLength);

                if (heightMap[x, z] < 0.80f && heightMap[x, z] > 0.25f)
                {
                    break;
                }
            }

            switch (traits.species)
            {
                case Species.Rabbit:
                    if (spawnWithManualActions)
                    {
                        trait.fcmHandler = new MockFCMHandler(new RabbitFCMHandler(FCMFactory.RabbitFCM()));
                    }
                    else
                    {
                        trait.fcmHandler = new RabbitFCMHandler(FCMFactory.RabbitFCM());
                    }
                    break;
                case Species.Fox:
                    if (spawnWithManualActions)
                    {
                        trait.fcmHandler = new MockFCMHandler(new FoxFCMHandler(FCMFactory.FoxFCM()));
                    }
                    else
                    {
                        trait.fcmHandler = new FoxFCMHandler(FCMFactory.FoxFCM());
                    }
                    break;
                default:
                    break;
            }

            OrganismFactory.CreateAnimal(trait, new Vector3(x, terrainKernal.amplifier * terrainKernal.animCurve.Evaluate(heightMap[x, z]), z));
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
        Time.fixedDeltaTime = 0.02f * gameSpeed;
        spawnWithManualActions = _spawnWithManualActions;
        animalCanDie = _animalCanDie;
    }


}
