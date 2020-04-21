using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class SimulationController : MonoBehaviour
{
    //Singleton
    private static SimulationController _instance;

    //public int Test = SimulationSettings.Instance.TEST_VARIABLE;
    [SerializeField]
    public SimulationSettings settings = new SimulationSettings();


    //The number of animals that should spawn each round
    protected Dictionary<Species, int> nAnimals = new Dictionary<Species, int>();

    //Current animals that are alive in the simulation
    protected Dictionary<Species, IList<Animal>> animalsInSimulation = new Dictionary<Species, IList<Animal>>();

    //The standard traits that the animals should spawn with (unless they evolve)
    protected Dictionary<Species, AnimalTraits> baseTraits = new Dictionary<Species, AnimalTraits>();

    //The animals that should spawn during the next round of the simulation
    protected Dictionary<Species, AnimalTraits[]> animalsToSpawn = new Dictionary<Species, AnimalTraits[]>();
    

    //Spawn location specific
    protected System.Random random = new System.Random();
    private TerrainKernal terrainKernal;
    private float[,] heightMap;
    private int sideLength;

    //The main genetic operators used for breeding and mutation
    //public static ICrossover CROSSOVER_OPERATOR = BlendCrossover.Instance;
    //public static IMutation MUTATION_OPERATOR = NoMutation.Instance;

    public static ICrossover CROSSOVER_OPERATOR = AlwaysSameCrossover.Instance;
    public static IMutation MUTATION_OPERATOR = GaussianMutation.Instance;

    // start values
    double maxSize = 3;
    private double startSizeFactor = 0.5;
    private double startThirst = 0.3;


    protected virtual void Awake()
    {
        
        if (_instance == null)
        {

            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this);
        }
    }
    protected virtual void Start()
    {
        GameObject gameMaster = GameObject.Find("Game Master");
        terrainKernal = gameMaster.GetComponent<TerrainKernal>();
        ComponentNavigator.LoadData(terrainKernal.GetPuddleList());
        heightMap = terrainKernal.GetHeightMap();
        sideLength = terrainKernal.resolution;

        InitAmountOfOrganisms();
        InitBaseTraits();
        //Init relevant lists
        InitLists();
        StartSimulation();
    }

    protected virtual void StartSimulation()
    {
        foreach (Species s in nAnimals.Keys)
        {
            animalsToSpawn[s] = InitialPopulation(nAnimals[s], s);
        }
        SpawnPlants();
        SpawnAnimals();
    }

    private void InitAmountOfOrganisms()
    {
        //nAnimals[Species.Plant] = nPlants;
        nAnimals[Species.Rabbit] = settings.nRabbits;
        nAnimals[Species.Fox] = settings.nFoxes;
    }

    protected virtual void InitLists()
    {
        foreach (Species s in nAnimals.Keys)
        {
            animalsInSimulation[s] = new List<Animal>();
            animalsToSpawn[s] = new AnimalTraits[0];
        }
    }

    // Random location on the map
    protected Vector3 GetSpawnLocation()
    {
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
        		Vector3 location = new Vector3(x, terrainKernal.amplifier * terrainKernal.animCurve.Evaluate(heightMap[x, z]), z);
		NavMeshHit hit = new NavMeshHit();
		NavMesh.SamplePosition(location, out hit, 100, NavMesh.AllAreas);
		return hit.position;
    }

    private void SpawnPlants()
    {
        for(int i = 0; i < settings.nPlants; i++)
        {
            Vector3 spawnPoint = GetSpawnLocation();
            SpawnPlant(spawnPoint);
        }
    }

    protected void SpawnAnimals()
    {
        foreach (Species s in animalsToSpawn.Keys)
        {
            foreach (AnimalTraits traits in animalsToSpawn[s])
            { 
                Vector3 spawnPoint = GetSpawnLocation();
                SpawnAnimal(traits, spawnPoint);
            }
        }
    }

    protected void SpawnAnimal(AnimalTraits traits, Vector3 spawnPoint)
    {
        OrganismFactory.CreateAnimal(traits, spawnPoint, startSizeFactor * maxSize, startThirst);
    }

    protected void SpawnPlant(Vector3 spawnPoint)
    {
        OrganismFactory.CreatePlant(3, spawnPoint);
    }


    public void EndSimulation()
    {

    }

    /**
     * The initial population that is to be spawned when the simulation starts
     */
    public AnimalTraits[] InitialPopulation(int amount, Species s)
    {
        AnimalTraits[] population = new AnimalTraits[amount];

        for(int i = 0; i < amount; i++)
        {
            population[i] = baseTraits[s].Duplicate();
        }

        return population;
    }

    /**
     * The traits that the initialpopulation should have
     */
    private void InitBaseTraits()
    {
        String[] rabbitArr = new String[] {"Rabbit" };
        String[] foxArr = new String[] { "Fox" };
        String[] plantArr = new String[] { "Plant" };
        String[] emptyArr = new string[] { "" };

        AnimalTraits rabbitTraits = InitTraitsFromSettings(settings.rabbit);
        rabbitTraits.species = Species.Rabbit;
        rabbitTraits.fcmHandler = new RabbitFCMHandler(FCMFactory.RabbitFCM());
        rabbitTraits.diet = plantArr;
        rabbitTraits.foes = foxArr;
        rabbitTraits.mates = rabbitArr;

        AnimalTraits foxTraits = InitTraitsFromSettings(settings.fox);
        foxTraits.species = Species.Fox;
        foxTraits.fcmHandler = new FoxFCMHandler(FCMFactory.FoxFCM());
        foxTraits.diet = rabbitArr;
        foxTraits.foes = emptyArr;
        foxTraits.mates = foxArr;
    
        baseTraits[Species.Rabbit] = rabbitTraits;
        baseTraits[Species.Fox] = foxTraits;
    }

    private AnimalTraits InitTraitsFromSettings(AnimalSettings s)
    {
        return new AnimalTraits(0, s.maxSize, s.dietFactor, s.nChildren, s.speed, s.heatTimer, s.sightLength, s.smellRadius, null, null, null, null);
    }
    // register spawned animal
    public virtual void Register(Animal animal)
    {
        Species species = animal.GetTraits().species;
        animalsInSimulation[species].Add(animal);
        TraitLogger.Register(animal);
    }

    // register animal death
    public virtual void Unregister(Animal animal)
    {
        AnimalTraits traits = animal.GetTraits();
        animalsInSimulation[traits.species].Remove(animal);
        TraitLogger.Unregister(animal);
    }

    private void OnValidate()
    {
        Time.timeScale = 1;
        //Ändras 0.25f så måste det ändras i ticktimer också atm.
        Time.fixedDeltaTime = 0.25f / settings.gameSpeed;
    }

    public static SimulationController Instance()
    {
        return _instance;
    }

    public void RegisterBirth(Species s)
    {
        TraitLogger.RegisterBirth(s);
    }
}
