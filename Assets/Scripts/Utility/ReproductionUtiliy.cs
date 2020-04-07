using System;
using UnityEngine;

public static class ReproductionUtility
{
    //private static double STD_DEVIATION_FACTOR = 0.2;
    //private static double MUTATION_CHANCE = 0.2;
    private static ICrossover CROSSOVER_OPERATOR = SimulationController.CROSSOVER_OPERATOR;
    private static IMutation MUTATION_OPERATOR = SimulationController.MUTATION_OPERATOR;




    public static RangedDouble ReproduceRangedDouble(RangedDouble geneA, RangedDouble geneB)
    {
        RangedDouble mutation = CROSSOVER_OPERATOR.Crossover(geneA, geneB);
        mutation = MUTATION_OPERATOR.Mutate(mutation);
        return mutation;
    }


    /*public static RangedInt ReproduceRangedInt(RangedInt geneA, RangedInt geneB)
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
    */
    public static AnimalTraits ReproduceAnimal(AnimalTraits traitsA, AnimalTraits traitsB)
    {
        SimulationController sc = SimulationController.Instance();
        Species species = traitsA.species;
        double maxSize = sc.EvovleMaxSize ? ReproduceRangedDouble(traitsA.maxSize, traitsB.maxSize).GetValue(): traitsA.maxSize.GetValue();
        double dietFactor = sc.EvolveDietFactor ? ReproduceRangedDouble(traitsA.dietFactor, traitsB.dietFactor).GetValue() : traitsA.dietFactor.GetValue();
        double nChildren = sc.EvolveNChildren ? ReproduceRangedDouble(traitsA.nChildren, traitsB.nChildren).GetValue() : traitsA.nChildren.GetValue();
        double infantFactor = sc.EvolveInfantFactor ? ReproduceRangedDouble(traitsA.infantFactor, traitsB.infantFactor).GetValue() : traitsA.infantFactor.GetValue();
        double speed = sc.EvolveSpeed ? ReproduceRangedDouble(traitsA.speed, traitsB.speed).GetValue(): traitsA.speed.GetValue();
        double heatTimer = sc.EvolveHeatTimer ? ReproduceRangedDouble(traitsA.heatTimer, traitsB.heatTimer).GetValue() : traitsA.heatTimer.GetValue();
        double sightLength = sc.EvolveSightLength ? ReproduceRangedDouble(traitsA.sightLength, traitsB.sightLength).GetValue() : traitsA.sightLength.GetValue();
        double smellRadius = sc.EvovleSmellRadius ? ReproduceRangedDouble(traitsA.smellRadius, traitsB.smellRadius).GetValue() : traitsA.smellRadius.GetValue();
        FCMHandler fcmHandler = sc.EvolveFcm ? traitsA.fcmHandler.Reproduce(traitsB.fcmHandler) : FCMHandlerFactory.getFCMHandlerSpecies(FCMFactory.getSpeciesFCM(species), species);

        AnimalTraits child = new AnimalTraits(species, maxSize, dietFactor, nChildren, infantFactor, speed, heatTimer, sightLength, smellRadius, fcmHandler, traitsA.diet, traitsA.foes, traitsA.mates);

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
