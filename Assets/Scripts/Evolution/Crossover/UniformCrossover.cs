using System;

public class UniformCrossover : SingletonBase<UniformCrossover>, ICrossover
{
    Random random = new Random();

    public RangedDouble Crossover(RangedDouble geneA, RangedDouble geneB)
    {
        if (random.Next(2) == 1)
        {
            return geneA.Duplicate();
        }
        return geneB.Duplicate();
    }
}



