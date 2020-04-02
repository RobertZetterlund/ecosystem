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
    private int finishedRounds = 0;

    public int parentsPerRound = 10;

    protected override void Start()
    {
        speciesToEvolve.Add(Species.Rabbit);
        speciesToEvolve.Add(Species.Fox);
        roundTimer = new Timer(5);

        base.Start();
    }

    private void Update()
    {
        if(roundTimer.IsDone()) {
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
        roundTimer.Reset();
        roundTimer.Start();

        List<AnimalTraits>[] traitsToBeSpawned = new List<AnimalTraits>[Species.GetValues(typeof(Species)).Length];

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


    }

    private void ResetFinishedTraits()
    {
        for (int i = 0; i < finishedTraits.Length; i++)
        {
            finishedTraits[i].Clear();
        }
    }

    private List<AnimalTraits> NewGeneration(Species s)
    {
        List<AnimalTraits> parents = new List<AnimalTraits>();
        List<AnimalTraits> population = new List<AnimalTraits>();
        foreach (TraitsComparable tc in finishedTraits[(int)s].Keys)
        {
            population.Add(tc.traits);
        }
        parents = SelectParents(population, finishedTraits[(int)s].Values.ToList(), parentsPerRound);

        return BreedChildren(parents, nOrganisms[(int)s]);
    }

    private List<AnimalTraits> BreedChildren(List<AnimalTraits> parents, int amount)
    {
        List<AnimalTraits> children = new List<AnimalTraits>();
        if (amount < 2)
            throw new Exception("Can't breed with less than 2 parents");

        while(true)
        {
            for (int parent1Index = 0; parent1Index < parents.Count; parent1Index++)
            {
                int parent2Index = random.Next(parents.Count);

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

                children.Add(ReproductionUtility.ReproduceAnimal(parent1, parent2));

                if (children.Count == amount)
                    return children;
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
    }

    private double CalculateFitness(Animal animal)
    {
        return animal.GetTimeAlive();
    }

    private int RouletteWheelSelection<T> (IList<T> pool, IList<double> fitness)
    {
        double totalFitness = 0;
        foreach (double _fitness in fitness)
        {
            totalFitness += _fitness;
        }

        int populationSize = pool.Count;

        double randomFitness = random.NextDouble() * totalFitness;
        int idx = -1;
        int mid;
        int first = 0;
        int last = populationSize - 1;
        mid = (last - first) / 2;

        //  ArrayList's BinarySearch is for exact values only
        //  so do this by hand.
        while (idx == -1 && first <= last)
        {
            if (randomFitness < fitness[mid])
            {
                last = mid;
            }
            else if (randomFitness > fitness[mid])
            {
                first = mid;
            }
            mid = (first + last) / 2;

            //  lies between i and i+1
            if ((last - first) <= 1)
                idx = last;
        }
        return idx;
    }

    private List<T> SelectParents<T>(IList<T> pool, IList<double> fitness, int amount)
    {
        List<T> newGeneration = new List<T>();
        for(int i = 0; i < amount && pool.Count != 0; i++)
        {
            int selectedIndex = RouletteWheelSelection<T>(pool, fitness);
            newGeneration.Add(pool[selectedIndex]);
            pool.RemoveAt(selectedIndex);
            fitness.RemoveAt(selectedIndex);
        }
        return newGeneration;
    }

}

