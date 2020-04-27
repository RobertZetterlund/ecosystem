using System;
using UnityEngine;

[Serializable]
public class SimulationSettings
{
    //Available to the editor
    public int nPlants = 40;
    public int nRabbits = 40;
    public int nFoxes = 0;
    [Space(20)]
    [Range(0.1f, 100)]
    public float gameSpeed = 1;
    public bool animalsCanDie = true;
    public float cdt = 0.1f;
    public int plantSize = 1;

    public AnimalSettings rabbit = new RabbitSettings();
    public AnimalSettings fox = new FoxSettings();



    public enum MutationSettings
    {
        GA,
        ES,
        Search
    }

    public MutationSettings mutationSettings = MutationSettings.GA;


    /*public enum CrossoverOperator
    {
        BLX_ALPHA,
        UNIFORM,
        ALWAYS_SAME
    }

    public enum MutationOperator
    {
        GAUSSIAN,
        NO_MUTATION
    }


    public CrossoverOperator CROSSOVER_OPERATOR = CrossoverOperator.ALWAYS_SAME;
    public MutationOperator MUTATION_OPERATOR = MutationOperator.GAUSSIAN;*/

    [Header("Evolution")]
    public bool evolveRabbit = true;
    public bool evolveFox = true;

    public float parentsPercentage = 0.2f;
    public float topPerformersPercentage = 0.05f;
    public int roundTime = 100000;

    public double std_dev = 0.5;
    public double mutation_chance = 0.05;

    [HideInInspector]
    public bool EvovleMaxSize = false, EvolveDietFactor = false, EvolveNChildren = false, EvolveInfantFactor = false,
    EvolveSpeed = false, EvolveHeatTimer = false, EvolveSightLength = false, EvovleSmellRadius = false;

    public bool EvolveFcm = true;

}

