using System;

class TraitsComparable : IComparable<TraitsComparable>
{
    public double fitness;
    public AnimalTraits traits;

    public TraitsComparable(AnimalTraits traits, double fitness)
    {
        this.fitness = fitness;
        this.traits = traits;
    }


    public int CompareTo(TraitsComparable other)
    {
        return (fitness > other.fitness) ? 1 : -1;
    }
}

