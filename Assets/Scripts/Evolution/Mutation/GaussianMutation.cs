using System;


class GaussianMutation : SingletonBase<GaussianMutation>, IMutation
{
    private static double STD_DEVIATION_FACTOR = 0.2;

    public RangedDouble Mutate(RangedDouble gene)
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

        mutation = MathUtility.Clamp(mutation, lower, upper);
        return new RangedDouble(mutation, lower, upper);
    }
}