using System;
using UnityEngine;

public static class ReproductionUtility
{
    private static double STD_DEVIATION_FACTOR = 0.2;
    private static double MUTATION_CHANCE = 0.7;
    private static System.Random random = MathUtility.random;


    // Crossover and mutate
    public static RangedDouble ReproduceRangedDouble(RangedDouble geneA, RangedDouble geneB)
    {
        RangedDouble crossedGene = Crossover(geneA, geneB);

        if (random.NextDouble() < MUTATION_CHANCE)
        {
            return MutateRangedDouble(crossedGene);
        }
        return crossedGene;
    }

    // Crossover and mutate
    public static RangedDouble ReproduceRangedDouble(RangedDouble gene)
    {
        if (random.NextDouble() < MUTATION_CHANCE)
        {
            return MutateRangedDouble(gene);
        }
        return gene;
    }

    public static RangedInt ReproduceRangedInt(RangedInt geneA, RangedInt geneB)
    {
        RangedInt crossedGene = Crossover(geneA, geneB);

        if(random.NextDouble() < MUTATION_CHANCE)
        {
            return MutateRangedInt(crossedGene);
        }
        return crossedGene;
    }

    // Crossover and mutate
    public static bool ReproduceBool(bool geneA, bool geneB)
    {
        bool crossedGene = Crossover(geneA, geneB);

        if (random.NextDouble() < MUTATION_CHANCE)
        {
            return MutateBool(crossedGene);
        }
        return crossedGene;
    }

    // Crossover help function
    public static T Crossover<T>(T geneA, T geneB)
    {
        if (random.Next(2) == 1)
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

    private static RangedInt MutateRangedInt(RangedInt gene)
    {
        int offset = 1; // +
        if (random.NextDouble() < 0.5) // + or -
        {
            offset *= -1; // -
        }
        if (gene.Add(offset) != offset) // if fails to add offset, then value was == lower or == higher
        {
            gene.Add(-offset); // then add offset in the oppposite direction
        }
        return new RangedInt(gene.GetValue(), gene.GetLower(), gene.GetUpper());
    }

    // Mutation help function
    private static bool MutateBool(bool gene)
    {
        return !gene;
    }
}
