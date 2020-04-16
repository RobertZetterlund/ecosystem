using System;
using UnityEngine;

public class BlendCrossover : SingletonBase<BlendCrossover>, ICrossover
{
	public double Alpha { get; } = 0.5;

	/**
     * Crosses two genetic constilations
     * This method assumes that the same genes resides on the same indexes in the array
     */
	public RangedDouble Crossover(RangedDouble geneA, RangedDouble geneB)
	{
		if (geneA.GetLower() != geneB.GetLower() || geneA.GetUpper() != geneB.GetUpper())
			throw new Exception("Tried to crossover genes with different bounds");

		double val1 = geneA.GetValue();
		double val2 = geneB.GetValue();

		double lower = Math.Min(val1, val2);
		double upper = Math.Max(val1, val2);

		double exploitation = Math.Abs(upper - lower);
		double exploration = Alpha * exploitation;
		double mutation = MathUtility.RandomUniform(lower - exploration, upper + exploration);
		mutation = MathUtility.Clamp(mutation, geneA.GetLower(), geneB.GetUpper());
		return new RangedDouble(mutation, geneA.GetLower(), geneB.GetUpper());
	}
}

