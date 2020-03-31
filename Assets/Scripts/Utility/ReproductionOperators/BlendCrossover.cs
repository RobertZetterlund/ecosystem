using System;

public class BlendCrossover : ICrossover
{
    public double alpha { get; } = 0.5;

    private static BlendCrossover instance = new BlendCrossover();

    private BlendCrossover()
    {

    }

    public static BlendCrossover GetInstance()
    {
        return instance;
    }

    public double Crossover(double val1, double val2)
    {
        double lower = Math.Min(val1, val2);
        double upper = Math.Max(val1, val2);

        double exploitation = Math.Abs(upper - lower);
        double exploration = alpha * exploitation;
        return MathUtility.RandomUniform(lower - exploration, upper + exploration);
    }
}

