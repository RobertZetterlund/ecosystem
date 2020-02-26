using System;
using UnityEngine;

public class GameController
{
    private static double STD_DEVIATION_FACTOR = 0.2;
    private static double MUTATION_CHANCE = 0.2;

	public GameController()
	{
	}


    // Crossover and mutate
    private static RangedDouble ReproduceRangedDouble(RangedDouble geneA, RangedDouble geneB)
    {
        RangedDouble crossedGene = Crossover(geneA, geneB);

        System.Random random = new System.Random();
        if (random.NextDouble() < MUTATION_CHANCE)
        {
            return MutateRangedDouble(crossedGene);
        }
        return crossedGene;
    }

    // Crossover and mutate
    private static bool ReproduceBool(bool geneA, bool geneB)
    {
        bool crossedGene = Crossover(geneA, geneB);

        System.Random random = new System.Random();
        if (random.NextDouble() < MUTATION_CHANCE)
        {
            return MutateBool(crossedGene);
        }
        return crossedGene;
    }

    // Crossover help function
    private static T Crossover<T>(T geneA, T geneB)
    {
        System.Random rand = new System.Random();
        if (rand.Next(2) == 1)
        {
            return geneA;
        }
        return geneB;
    }

    // Mutation help function
    private static RangedDouble MutateRangedDouble(RangedDouble gene)
    {
        double value = gene.GetValue();
        double lower = gene.GetLower();
        double upper = gene.GetUpper();
        double mutation;
        double difference;
        double amountInsideRange;
        do // randomize new value until it is within the allowed range
        {
            double deviation = Math.Min(Math.Abs(value - lower), Math.Abs(value - upper)) 
                * STD_DEVIATION_FACTOR;
            mutation = MathUtility.RandomGaussian(value, deviation);
            difference = mutation - gene.GetValue();
            amountInsideRange = gene.Add(difference);
        } while (amountInsideRange != difference);

        return new RangedDouble(mutation, lower, upper);
    }

    // Mutation help function
    private static bool MutateBool(bool gene)
    {
        return !gene;
    }

    public void Reproduce(Animal a, Animal b)
    {
        double size = ReproduceRangedDouble(a.GetSize(), b.GetSize()).GetValue();
        double dietFactor = ReproduceRangedDouble(a.GetDiet(), b.GetDiet()).GetValue();

        GameObject gameObject = new GameObject();
        Animal child = gameObject.AddComponent<Animal>();
        child.init(this, size, dietFactor);
        UnityEngine.Object.Instantiate(child);
    }





}
