using System;
using UnityEngine;

public static class ReproductionUtility
{
    //private static double STD_DEVIATION_FACTOR = 0.2;
    //private static double MUTATION_CHANCE = 0.2;
    private static ICrossover CROSSOVER_OPERATOR = BlendCrossover.GetInstance();




    public static RangedDouble ReproduceRangedDouble(RangedDouble geneA, RangedDouble geneB)
    {
        double mutation = CROSSOVER_OPERATOR.Crossover(geneA.GetValue(), geneB.GetValue());

        if (geneA.GetLower() == geneB.GetLower() && geneA.GetUpper() == geneB.GetUpper())
        {
            mutation = MathUtility.Clamp(mutation, geneA.GetLower(), geneB.GetUpper());
            return new RangedDouble(mutation, geneA.GetLower(), geneA.GetUpper());
        } 
        else
        {
            throw new Exception("Attempted to reproduce genes with different bounds");
        }
    }


    public static RangedInt ReproduceRangedInt(RangedInt geneA, RangedInt geneB)
    {
        int mutation = (int)CROSSOVER_OPERATOR.Crossover(geneA.GetValue(), geneB.GetValue());

        if (geneA.GetLower() == geneB.GetLower() && geneA.GetUpper() == geneB.GetUpper())
        {
            mutation = (int)MathUtility.Clamp(mutation, geneA.GetLower(), geneB.GetUpper());
            return new RangedInt(mutation, geneA.GetLower(), geneA.GetUpper());
        }
        else
        {
            throw new Exception("Attempted to reproduce genes with different bounds");
        }
    }

    public static AnimalTraits ReproduceAnimal(AnimalTraits traitsA, AnimalTraits traitsB)
    {
        Species species = traitsA.species;
        double maxSize = ReproduceRangedDouble(traitsA.maxSize.Duplicate(), traitsB.maxSize.Duplicate()).GetValue();
        double dietFactor = ReproduceRangedDouble(traitsA.dietFactor.Duplicate(), traitsB.dietFactor.Duplicate()).GetValue();
        int nChildren = ReproduceRangedInt(traitsA.nChildren.Duplicate(), traitsB.nChildren.Duplicate()).GetValue();
        double infantFactor = ReproduceRangedDouble(traitsA.infantFactor.Duplicate(), traitsB.infantFactor.Duplicate()).GetValue();
        double growthFactor = ReproduceRangedDouble(traitsA.growthFactor.Duplicate(), traitsB.growthFactor.Duplicate()).GetValue();
        double speed = ReproduceRangedDouble(traitsA.speed.Duplicate(), traitsB.speed.Duplicate()).GetValue();
        double heatTimer = ReproduceRangedDouble(traitsA.heatTimer.Duplicate(), traitsB.heatTimer.Duplicate()).GetValue();
        FCMHandler fcmHandler = traitsA.fcmHandler.Reproduce(traitsB.fcmHandler);

        AnimalTraits child = new AnimalTraits(species, maxSize, dietFactor, nChildren, infantFactor, growthFactor, speed, heatTimer, fcmHandler);

        return child;
    }


    /*
    // Crossover and mutate
    public static RangedDouble ReproduceRangedDouble(RangedDouble gene)
    {

        if (random.NextDouble() < MUTATION_CHANCE)
        {
            return MutateRangedDouble(gene);
        }
        return gene;
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

    // Crossover and mutate
    /*public static bool ReproduceBool(bool geneA, bool geneB)
    {
        bool crossedGene = Crossover(geneA, geneB);

        System.Random random = new System.Random();
        if (random.NextDouble() < MUTATION_CHANCE)
        {
            return MutateBool(crossedGene);
        }
        return crossedGene;
    }*/

    // Crossover and mutate
    /*public static RangedDouble ReproduceRangedDouble(RangedDouble geneA, RangedDouble geneB)
    {
        RangedDouble crossedGene = Crossover(geneA, geneB);

        System.Random random = new System.Random();
        if (random.NextDouble() < MUTATION_CHANCE)
        {
            return MutateRangedDouble(crossedGene);
        }
        return crossedGene;
    }*/

    /*public static RangedInt ReproduceRangedInt(RangedInt geneA, RangedInt geneB)
    {
        RangedInt crossedGene = Crossover(geneA, geneB);

        System.Random random = new System.Random();
        if(random.NextDouble() < MUTATION_CHANCE)
        {
            return MutateRangedInt(crossedGene);
        }
        return crossedGene;
    }*/

}
