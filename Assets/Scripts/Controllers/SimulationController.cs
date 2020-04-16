using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

abstract class SimulationController : MonoBehaviour
{
	//Singleton
	private static SimulationController _instance;

	//Available to the editor
	public int nPlants;
	public int nRabbits;
	public int nFoxes;
	[Range(0.1f, 100)]
	public float gameSpeed = 1;
	public bool animalsCanDie = true;
	public bool randomiseRabbitFCM;
	public bool randomiseFoxFCM;

	public bool EvovleMaxSize = true, EvolveDietFactor = true, EvolveNChildren = true, EvolveInfantFactor = true,
		EvolveSpeed = true, EvolveHeatTimer = true, EvolveSightLength = true, EvovleSmellRadius = true, EvolveFcm = true;

	protected List<Animal>[] organisms = new List<Animal>[Species.GetValues(typeof(Species)).Length];
	protected int[] nOrganisms = new int[Species.GetValues(typeof(Species)).Length];
	protected AnimalTraits[] baseTraits = new AnimalTraits[Species.GetValues(typeof(Species)).Length];


	//Spawn location specific
	protected System.Random random = new System.Random();
	private TerrainKernal terrainKernal;
	private float[,] heightMap;
	private int sideLength;

	public static ICrossover CROSSOVER_OPERATOR = UniformCrossover.Instance;
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
		InitLists();
		InitAmountOfOrganisms();
		InitBaseTraits();
		StartSimulation();
	}

	protected abstract void StartSimulation();

	protected virtual void InitLists()
	{
		for (int i = 0; i < organisms.Length; i++)
			organisms[i] = new List<Animal>();
	}

	private void InitAmountOfOrganisms()
	{
		nOrganisms[(int)Species.Plant] = nPlants;
		nOrganisms[(int)Species.Rabbit] = nRabbits;
		nOrganisms[(int)Species.Fox] = nFoxes;
	}

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

	protected void SpawnOrganisms()
	{
		for (int specie = 0; specie < organisms.Length; specie++)
		{
			for (int i = 0; i < nOrganisms[specie]; i++)
			{
				Vector3 spawnPoint = GetSpawnLocation();
				if (specie == (int)Species.Plant)
				{
					SpawnPlant(spawnPoint);
				}
				else
				{
					AnimalTraits traits = baseTraits[specie].Duplicate();
					SpawnAnimal(traits, spawnPoint);
				}
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

	private void InitBaseTraits()
	{
		String[] rabbitArr = new String[] { "Rabbit" };
		String[] foxArr = new String[] { "Fox" };
		String[] plantArr = new String[] { "Plant" };
		String[] emptyArr = new string[] { "" };

		double smellRadius = 25;
		AnimalTraits rabbitTraits = new AnimalTraits(Species.Rabbit, maxSize, 0, 2.1, 4, 13, 30, smellRadius, new RabbitFCMHandler(FCMFactory.RabbitFCM()), plantArr, foxArr, rabbitArr);
		AnimalTraits foxTraits = new AnimalTraits(Species.Fox, maxSize, 1, 2, 4, 13, 30, smellRadius, new FoxFCMHandler(FCMFactory.FoxFCM()), rabbitArr, emptyArr, foxArr);

		baseTraits[(int)Species.Rabbit] = rabbitTraits;
		baseTraits[(int)Species.Fox] = foxTraits;
	}

	// register new animal
	public virtual void Register(Animal animal)
	{
		Species species = animal.GetTraits().species;
		organisms[(int)species].Add(animal);
		TraitLogger.Register(animal);
	}

	// register animal death, spawn new ones if all died
	public virtual void Unregister(Animal animal)
	{
		AnimalTraits traits = animal.GetTraits();
		organisms[(int)traits.species].Remove(animal);
		TraitLogger.Unregister(animal);
	}

	private void OnValidate()
	{
		Time.timeScale = 1;
		//Ändras 0.25f så måste det ändras i ticktimer också atm.
		Time.fixedDeltaTime = 0.25f / gameSpeed;
	}

	public static SimulationController Instance()
	{
		return _instance;
	}

}

