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

    private double parentPercentage;
    private double roundTime;
    private int finishedRounds = 0;
    private int evolvingAnimalsAlive = 0;

    private static ISelection SELECTION_OPERATOR = RouletteSelection.Instance;


    protected override void Start()
    {
        parentPercentage = settings.parentsPercentage;
        roundTime = settings.roundTime;
        if(settings.evolveRabbit)
            speciesToEvolve.Add(Species.Rabbit);
        if(settings.evolveFox)
            speciesToEvolve.Add(Species.Fox);
        roundTimer = new Timer(settings.roundTime);
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
        TraitLogger.StartNewRound();
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

        AnimalTraits avgAnimal = GenerateAverageAnimal(finishedTraits[s], s);


        //Minimum 2 parents
        int parentsPerRound = Math.Max(2,(int)(parentPercentage * nAnimals[s])); 
        AnimalTraits[] parents = SELECTION_OPERATOR.Select(population.ToArray(), finishedTraits[s].Values.ToArray<double>(), parentsPerRound);

        int nTop = 5;
        AnimalTraits[] children = BreedChildren(parents, nAnimals[s] - nTop);
        AnimalTraits[] topPerformers = BestSelection.Instance.Select(population.ToArray(), finishedTraits[s].Values.ToArray<double>(), nTop);
        return children.Concat(topPerformers).ToArray();
    }

    private AnimalTraits GenerateAverageAnimal(SortedList<TraitsComparable, double> weightedTraits, Species specie)
    {

        double totalFitness = weightedTraits.Values.Sum();

        // c_____ means combined/cumulative
        double cMaxSize = 0;
        double cDietFactor = 0;
        double cNChildren = 0;
        double cSpeed = 0;
        double cHeatTimer = 0;
        double cSightLength = 0;
        double cSmellRadius = 0;

        // this fcm is emtpy, but will have cumulative weights.
        FCM cFCM = FCMFactory.GetBaseFCM();

        // Extract animaltrait to fix compiler complaining when extracting diet,foes and mates.
        AnimalTraits currTrait = weightedTraits.Keys.First().traits;

        foreach(KeyValuePair<TraitsComparable, double> kvP in weightedTraits)
        {
            currTrait = kvP.Key.traits;
            double currFitness = kvP.Value;
            double adjustedFitness = currFitness / totalFitness;
            FCMHandler currHandler = currTrait.fcmHandler;


            FCM currFCM = currHandler.GetFCM();

            // average fcm
            for(int _from = 0; _from < currFCM.NOFields; _from++)
            {
                for(int _to=0; _to < currFCM.NOFields; _to++)
                {
                    cFCM.weights[_from, _to] += currFCM.weights[_from, _to] * adjustedFitness;
                }
            }


            // average traits
            cMaxSize += currTrait.maxSize.GetValue() * currFitness;
            cDietFactor += currTrait.dietFactor.GetValue() * currFitness;
            cNChildren += currTrait.nChildren.GetValue() * currFitness;
            cSpeed += currTrait.speed.GetValue() * currFitness;
            cHeatTimer += currTrait.heatTimer.GetValue() * currFitness;
            cSightLength += currTrait.sightLength.GetValue() * currFitness;
            cSmellRadius += currTrait.smellRadius.GetValue() * currFitness;
        }


        // Create fcm based on specie
        FCMHandler cFCMHandler;
        switch (specie)
        {
            case Species.Rabbit: cFCMHandler = new RabbitFCMHandler(cFCM); break;

            case Species.Fox: cFCMHandler = new FoxFCMHandler(cFCM); break;

            default: cFCMHandler = new RabbitFCMHandler(cFCM); break;
        }

        AnimalTraits averagedTraits = new AnimalTraits(specie, cMaxSize, cDietFactor, cNChildren, cSpeed, cHeatTimer, cSightLength, cSmellRadius, cFCMHandler, currTrait.diet, currTrait.foes, currTrait.mates);

        return averagedTraits;
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

