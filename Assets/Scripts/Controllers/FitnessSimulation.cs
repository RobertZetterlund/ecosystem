using Assets.Scripts;
using System;
using System.Collections.Generic;

class FitnessSimulation : SimulationController
{

    //The double is meant as the fitness function, so you can tie a trait to the fitness of the trait
    private List<(AnimalTraits, double)>[] allTraits = new List<(AnimalTraits, double)>[Species.GetValues(typeof(Species)).Length];

    public void EndRound()
    {
        throw new NotImplementedException();
    }

    public void StartRound()
    {
        List<AnimalTraits>[] traitsToBeSpawned = GenerateNewGeneration();
    }

    private List<AnimalTraits>[] GenerateNewGeneration()
    {
        List<AnimalTraits>[] traits = SelectTraits(allTraits);
        return traits;
    }

    private List<AnimalTraits>[] SelectTraits(List<(AnimalTraits, double)>[] allTraits)
    {
        throw new NotImplementedException();
    }

    protected override void StartSimulation()
    {
        StartRound();
    }

    protected override void InitLists()
    {
        base.InitLists();
        for (int i = 0; i < organisms.Length; i++)
            allTraits[i] = new List<(AnimalTraits, double)>();
    }

    protected override void Unregister(Animal animal)
    {
        base.Register(animal);

        AnimalTraits traits = animal.GetTraits();
        double fitness = CalculateFitness(animal);
        allTraits[(int)traits.species].Add((traits, fitness));
    }

    private double CalculateFitness(Animal animal)
    {
        return animal.GetTimeAlive();
    }

}

