﻿using Assets.Scripts;
using System;
using System.Collections.Generic;
using UnityEngine;

abstract class SimulationController : MonoBehaviour
{
    //Singleton
    private static SimulationController _instance;

    //Available to the editor
    public int nPlants;
    public int nRabbits;
    public int nFoxes;
    [Range(1f, 100)]
    public float gameSpeed = 1;
    public bool animalsCanDie = true;


    protected List<Animal>[] organisms = new List<Animal>[Species.GetValues(typeof(Species)).Length];
    protected int[] nOrganisms = new int[Species.GetValues(typeof(Species)).Length];
    protected  AnimalTraits[] baseTraits = new AnimalTraits[Species.GetValues(typeof(Species)).Length];
    

    //Spawn location specific
    private System.Random random = new System.Random();
    private TerrainKernal terrainKernal;
    private float[,] heightMap;
    private int sideLength;

    private void Awake()
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
    private void Start()
    {
        GameObject gameMaster = GameObject.Find("Game Master");
        terrainKernal = gameMaster.GetComponent<TerrainKernal>();
        heightMap = terrainKernal.GetHeightMap();
        sideLength = terrainKernal.resolution;
        StartSimulation();
    }

    private void Update()
    {

    }

    protected virtual void StartSimulation()
    {
        InitLists();
        InitAmountOfOrganisms();
        InitBaseTraits();
    }

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

    private Vector3 GetSpawnLocation()
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
        return new Vector3(x, terrainKernal.amplifier * terrainKernal.animCurve.Evaluate(heightMap[x, z]), z);
    }

    protected void SpawnOrganisms()
    {
        for (int specie = 0; specie < organisms.Length; specie++)
        {
            for(int i = 0; i < nOrganisms[specie]; i++)
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

    private void SpawnAnimal(AnimalTraits traits, Vector3 spawnPoint)
    {
        OrganismFactory.CreateAnimal(traits, spawnPoint);
    }

    private void SpawnPlant(Vector3 spawnPoint)
    {
        OrganismFactory.CreatePlant(100, spawnPoint);
    }


    public void EndSimulation()
    {

    }

    private void InitBaseTraits()
    {
        AnimalTraits rabbitTraits = new AnimalTraits(Species.Rabbit, 3, 0, 2, 0.1, 0.002, 3, 600, new RabbitFCMHandler(FCMFactory.RabbitFCM()));
        AnimalTraits foxTraits = new AnimalTraits(Species.Fox, 1, 1, 3, 0.1, 0.02, 3, 600, new RabbitFCMHandler(FCMFactory.RabbitFCM()));

        baseTraits[(int)Species.Rabbit] = rabbitTraits;
        baseTraits[(int)Species.Fox] = foxTraits;
    }

    // register new animal
    protected virtual void Register(Animal animal)
    {
        Species species = animal.GetTraits().species;
        organisms[(int)species].Add(animal);
    }

    // register animal death, spawn new ones if all died
    protected virtual void Unregister(Animal animal)
    {
        AnimalTraits traits = animal.GetTraits();
        organisms[(int)traits.species].Remove(animal);
    }

    private void OnValidate()
    {
        Time.timeScale = gameSpeed;
    }

    public static SimulationController Instance()
    {
        return _instance;
    }

}

