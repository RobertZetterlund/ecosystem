using System;

public class GameController
{
    private static double STD_DEVIATION_FACTOR = 0.2;
    private static double MUTATION_CHANCE = 0.2;

	public GameController()
	{
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

    public void Reproduce(Animal a, Animal b)
    {
        double size = ReproduceDouble(a.GetSize(), b.GetSize(), false);
        double dietFactor = ReproduceDouble(a.GetDiet(), b.GetDiet(), false);
        spawn(new Animal(this, size, dietFactor));
    }

    private static void spawn(Animal animal)
    {
        //todo
    }




}
