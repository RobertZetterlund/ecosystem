using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

class FitnessSimulation : SimulationController
{
    //The double is meant as the fitness function, so you can tie a trait to the fitness of the trait
    private Dictionary<Species, SortedList<TraitsComparable, double>> finishedTraits = new Dictionary<Species, SortedList<TraitsComparable, double>>();
    private List<Species> speciesToEvolve = new List<Species>();
    private Timer roundTimer;
    public int secondsPerRounds = 500;
    public int parentsPerRound = 10;

    private int finishedRounds = 0;
    private int evolvingAnimalsAlive = 0;

    private static ISelection SELECTION_OPERATOR = RouletteSelection.Instance;


    protected override void Start()
    {
        speciesToEvolve.Add(Species.Rabbit);
        speciesToEvolve.Add(Species.Fox);
        roundTimer = new Timer(secondsPerRounds);
        InitLists();
        base.Start();
    }

    private void Update()
    {
        if(roundTimer.IsDone() || evolvingAnimalsAlive == 0) {
            EndRound();
        }
    }

    protected override void InitLists()
    {
        base.InitLists();
        foreach(Species s in nAnimals.Keys)
        {
            finishedTraits[s] = new SortedList<TraitsComparable, double>();
        }
    }

    public void EndRound()
    {
        finishedRounds++;
        Debug.Log("Finished round + " + finishedRounds + " -  Fitness: " + totalFitness / totalCreatures + "    RoundTime: " + roundTimer.TimeSinceStart() + "    Animals alive at round finish: " + evolvingAnimalsAlive + "   Max alive: " + maxCreatures);
        totalCreatures = 0;
        totalFitness = 0;
        maxCreatures = 0;
        KillRemainingAnimals();
        PrepareNextRound();
        StartRound();
    }

    /**
     * Prepares everything for the next round, which mainly means creating new populations
     */
    private void PrepareNextRound()
    {
        evolvingAnimalsAlive = 0;

        foreach (Species s in nAnimals.Keys)
        {
            if (speciesToEvolve.Contains(s))
            {
                animalsToSpawn[s] = NewPopulation(s);
            }
        }

        ResetFinishedTraits();
    }

    /**
     * Kills off all animals still in the simulation
     */
    private void KillRemainingAnimals()
    {
        foreach (Species s in animalsToSpawn.Keys)
        {
            foreach (Animal animal in animalsInSimulation[s].ToList())
            {
                animal.Die(CauseOfDeath.ForceDeleted);
            }
        }
    }

    /**
     * Starts of the round by spawning animals and resetting timers
     */
    public void StartRound()
    {
        TraitLogger.ResetTimer();
        roundTimer.Reset();
        roundTimer.Start();
        SpawnAnimals();

    }

    // Reset list
    private void ResetFinishedTraits()
    {
        foreach (Species s in finishedTraits.Keys)
        {
            finishedTraits[s].Clear();
        }
    }

    /**
     * 
     * Generates a new population for a given species. This occurs in 3 steps.
     * 
     * 1: Parents are selected via the selection operator
     * 2: New children are bred from the parents
     * 3: The 5 top performing animals from the species gets to live during the next round as well
     * 
     */
    private AnimalTraits[] NewPopulation(Species s)
    {
        if (nAnimals[s] == 0)
            return new AnimalTraits[0];

        List<AnimalTraits> population = new List<AnimalTraits>();
        foreach (TraitsComparable tc in finishedTraits[s].Keys)
        {
            population.Add(tc.traits);
        }
        AnimalTraits[] parents = SELECTION_OPERATOR.Select(population.ToArray(), finishedTraits[s].Values.ToArray<double>(), parentsPerRound);

        int nTop = 5;
        AnimalTraits[] children = BreedChildren(parents, nAnimals[s] - nTop);
        AnimalTraits[] topPerformers = BestSelection.Instance.Select(population.ToArray(), finishedTraits[s].Values.ToArray<double>(), nTop);
        return children.Concat(topPerformers).ToArray();
    }


    private AnimalTraits[] BreedChildren(AnimalTraits[] parents, int amount)
    {
        AnimalTraits[] children = new AnimalTraits[amount];
        if (parents.Length < 2)
            throw new Exception("Can't breed with less than 2 parents");

        int i = 0;
        while(true)
        {
            for (int parent1Index = 0; parent1Index < parents.Length; parent1Index++, i++)
            {
                if (i == amount)
                    return children;

                int parent2Index = random.Next(parents.Length);

                //No inbreeding allowed
                if (parent2Index == parent1Index)
                {
                    if (parent2Index == 0)
                        parent2Index += 1;
                    else
                        parent2Index -= 1;
                }

                AnimalTraits parent1 = parents[parent1Index];
                AnimalTraits parent2 = parents[parent2Index];

                children[i] = ReproductionUtility.ReproduceAnimal(parent1, parent2);
            }
        }
    }

    protected override void StartSimulation()
    {
        base.StartSimulation();
        roundTimer.Reset();
        roundTimer.Start();

    }

    // Called when an animal dies
    public override void Unregister(Animal animal)
    {
        base.Unregister(animal);
        AnimalTraits traits = animal.GetTraits();
        double fitness = CalculateFitness(animal);
        totalFitness += fitness;
        finishedTraits[traits.species].Add(new TraitsComparable(traits, fitness), fitness);

        if(speciesToEvolve.Contains(traits.species)) {
            evolvingAnimalsAlive -= 1;
        }
    }

    // Called when an animal is spawned
    public override void Register(Animal animal)
    {
        base.Register(animal);
        if (speciesToEvolve.Contains(animal.GetTraits().species))
        {
            evolvingAnimalsAlive += 1;
            if (evolvingAnimalsAlive > maxCreatures)
                maxCreatures = evolvingAnimalsAlive;
            totalCreatures += 1;
        }
    }

    double totalFitness = 0;
    double totalCreatures = 0;
    double maxCreatures = 0;
    // Evaluates the fitness of an animal. 
    private double CalculateFitness(Animal animal)
    {
        //return animal.GetTimeAlive();
        //Debug.Log("Fitness: " + roundTimer.TimeSinceStart());
        return Math.Pow(roundTimer.TimeSinceStart(), 2);
    }

}

