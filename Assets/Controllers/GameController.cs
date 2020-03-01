using System;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private static double STD_DEVIATION_FACTOR = 0.2;
    private static double MUTATION_CHANCE = 0.2;

    void Start()
    {
        // spawn first rabbit
        SpawnAnimal(AnimalType.Rabbit, 1, 1, new Vector3(5.83f, 1f, 14.36f));
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
        // TODO enable, hard coded reproduction
        /* 
        if (!(a.GetIsMale()^b.GetIsMale()))
        {
            // if same sex
            return;
        }
        */

        double size = ReproduceRangedDouble(a.GetSize(), b.GetSize()).GetValue();
        double dietFactor = ReproduceRangedDouble(a.GetDiet(), b.GetDiet()).GetValue();

        Vector3 mother;
        if (a.GetIsMale())
        {
            mother = b.transform.position;
        } else
        {
            mother = a.transform.position;
        }

        SpawnAnimal(a.GetType(), size, dietFactor, mother + new Vector3(0,0,-2));
    }

    private void SpawnAnimal(AnimalType type, double size, double dietFactor, Vector3 location)
    {
        Animal child = AnimalFactory.CreateAnimal(type);
        child.Init(this, size, dietFactor);
        child.transform.position = location;
        UnityEngine.Object.Instantiate(child);
    }




}
