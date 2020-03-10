
/**
 * Converts a value to be (linearly) between 0 and 1.
 * 
 */
class DistanceFuzzifier : IFuzzifier
{
    public float Fuzzify(float min, float max, float value)
    {
        if (value >= max)
            return 0;
        else if (value <= min)
            return 1;
        else
            return 1 - (value / (max - min));

    }
}
