using System;

class BestSelection : AbstractSelection<BestSelection>
{
    //Selects the highest value in the array
    public override int Select(double[] values)
    {
        int bestIndex = 0;
        double bestValue = 0;
        for(int i = 0; i < values.Length; i++)
        {
            if(values[i] > bestValue)
            {
                bestValue = values[i];
                bestIndex = i;
            }
        }
        return bestIndex;
    }
}

