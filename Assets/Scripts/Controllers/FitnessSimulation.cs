using System;
using System.Collections.Generic;
using System.Linq;

class FitnessSimulation : SimulationController
{

    //The double is meant as the fitness function, so you can tie a trait to the fitness of the trait
    //private List<(AnimalTraits, double)>[] allTraits = new List<(AnimalTraits, double)>[Species.GetValues(typeof(Species)).Length];
    private SortedList<TraitsComparable, double>[] finishedTraits = new SortedList<TraitsComparable, double>[Species.GetValues(typeof(Species)).Length];
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
        base.Start();
    }

    private void Update()
    {
        if(roundTimer.IsDone() || evolvingAnimalsAlive == 0) {
            EndRound();
            roundTimer.Reset();
            roundTimer.Start();
        }
    }

    public void EndRound()
    {
        finishedRounds++;
        for (int i = 0; i < organisms.Length; i++)
        {
            KillRemainingAnimals((Species)i);
        }
        StartRound();
    }

    private void KillRemainingAnimals(Species s)
    {
        foreach (Animal animal in organisms[(int)s].ToArray())
        {
            animal.Die(CauseOfDeath.ForceDeleted);
        }
    }

    public void StartRound()
    {

        evolvingAnimalsAlive = 0;
        AnimalTraits[][] traitsToBeSpawned = new AnimalTraits[Species.GetValues(typeof(Species)).Length][];

        for(int i = 0; i < organisms.Length; i++)
        {
            if (speciesToEvolve.Contains((Species)i))
            {
                traitsToBeSpawned[i] = NewGeneration((Species)i);
            }
        }

        for(int i = 0; i < organisms.Length; i++)
        {
            if (speciesToEvolve.Contains((Species)i))
            {
                foreach (AnimalTraits traits in traitsToBeSpawned[i])
                {
                    //Jag fuskar lite här för tillfället.
                    if (speciesToEvolve.Contains((Species)i))
                    {
                        SpawnAnimal(traits, GetSpawnLocation());
                    }
                }
            }
        }

        ResetFinishedTraits();
        TraitLogger.ResetTimer();

    }

    private void ResetFinishedTraits()
    {
        for (int i = 0; i < finishedTraits.Length; i++)
        {
            finishedTraits[i].Clear();
        }
    }

    private AnimalTraits[] NewGeneration(Species s)
    {
        if (nOrganisms[(int)s] == 0)
            return new AnimalTraits[0];

        List<AnimalTraits> population = new List<AnimalTraits>();
        foreach (TraitsComparable tc in finishedTraits[(int)s].Keys)
        {
            population.Add(tc.traits);
        }
        AnimalTraits[] parents = SELECTION_OPERATOR.Select(population.ToArray(), finishedTraits[(int)s].Values.ToArray<double>(), parentsPerRound);

        return BreedChildren(parents, nOrganisms[(int)s]);
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
        roundTimer.Reset();
        roundTimer.Start();
        if (finishedRounds == 0)
            SpawnOrganisms();
        else
            StartRound();
    }

    protected override void InitLists()
    {
        base.InitLists();
        for (int i = 0; i < organisms.Length; i++)
            finishedTraits[i] = new SortedList<TraitsComparable, double>();
    }

    public override void Unregister(Animal animal)
    {
        base.Unregister(animal);
        AnimalTraits traits = animal.GetTraits();
        double fitness = CalculateFitness(animal);
        finishedTraits[(int)traits.species].Add(new TraitsComparable(traits, fitness), fitness);

        if(speciesToEvolve.Contains(traits.species)) {
            evolvingAnimalsAlive -= 1;
        }
    }

    public override void Register(Animal animal)
    {
        base.Register(animal);
        if (speciesToEvolve.Contains(animal.GetTraits().species))
        {
            evolvingAnimalsAlive += 1;
        }
    }

    private double CalculateFitness(Animal animal)
    {
        //return animal.GetTimeAlive();
        return roundTimer.TimeSinceStart();
    }

}

