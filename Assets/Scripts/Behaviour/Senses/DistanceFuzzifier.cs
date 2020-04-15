using UnityEngine;

/**
 * Converts a value to be between 0 and 1 with the Sigmoid function.
 * 
 */
class DistanceFuzzifier : IFuzzifier
{
    public float Fuzzify(float min, float max, float value)
    {
        return 1/(1 + Mathf.Exp((10/max) * value - 4f));
    }
}
