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
    public float cdt = 0.3f;
    public float overallCostFactor = 1.5f;

    public AnimalSettings rabbit = new RabbitSettings();
    public AnimalSettings fox = new FoxSettings();



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
    public int roundTime = 1000;


    public bool EvovleMaxSize = true, EvolveDietFactor = true, EvolveNChildren = true, EvolveInfantFactor = true,
    EvolveSpeed = true, EvolveHeatTimer = true, EvolveSightLength = true, EvovleSmellRadius = true, EvolveFcm = true;

}

