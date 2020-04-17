using System;

class AlwaysSameCrossover : SingletonBase<AlwaysSameCrossover>, ICrossover
{

    public int geneToReturn = 1;

    public RangedDouble Crossover(RangedDouble geneA, RangedDouble geneB)
    {
        if (geneToReturn == 1)
        {
            return geneA.Duplicate();
        }
        return geneB.Duplicate();
    }
}

