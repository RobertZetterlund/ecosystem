using System;

public class GameController
{
    private static double BITE_FACTOR = 0.2;
    private static double STD_DEVIATION_FACTOR = 0.2;
    private static double MUTATION_CHANCE = 0.2;

	public GameController()
	{
	}

    public (double, ConsumptionType) Consume(Animal animal, IConsumable consumable)
    {
        // do eating calculations
        double biteSize = animal.GetSize() * BITE_FACTOR;
        double availableAmount = consumable.GetAmount();
        ConsumptionType type = consumable.GetConsumptionType();
        if (biteSize > consumable.GetAmount()) // trying to consume more than available
        {
            consumable.DecreaseAmount(availableAmount);
            return (availableAmount, type);
        } else // normal case
        {
            consumable.DecreaseAmount(biteSize);
            return (biteSize, type);
        }
    }

    // Crossover and mutate
    private static double ReproduceDouble(double geneA, double geneB, bool allowNegative)
    {
        double crossedGene = Crossover(geneA, geneB);

        Random random = new Random();
        if (random.NextDouble() < MUTATION_CHANCE)
        {
            return MutateDouble(crossedGene, allowNegative);
        }
        return crossedGene;
    }

    // Crossover and mutate
    private static bool ReproduceBool(bool geneA, bool geneB, bool allowNegative)
    {
        bool crossedGene = Crossover(geneA, geneB);

        Random random = new Random();
        if (random.NextDouble() < MUTATION_CHANCE)
        {
            return MutateBool(crossedGene);
        }
        return crossedGene;
    }

    // Crossover help function
    private static T Crossover<T>(T geneA, T geneB)
    {
        Random rand = new Random();
        if (rand.Next(2) == 1)
        {
            return geneA;
        }
        return geneB;
    }

    // Mutation help function
    private static double MutateDouble(double gene, bool allowNegative)
    {
        double mutated = MathUtility.RandomGaussian(gene, gene * STD_DEVIATION_FACTOR);
        if (!allowNegative)
        {
            mutated = Math.Abs(mutated);
        }
        // if gene == 0 we will cant mutate, but it's very unlikely to happen.
        return mutated;
    }

    // Mutation help function
    private static bool MutateBool(bool gene)
    {
        return !gene;
    }





}
