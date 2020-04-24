using System;
using System.Collections.Generic;
using System.Linq;

public class RouletteSelection : AbstractSelection<RouletteSelection>
{
	private readonly Random r = new Random();

	public override int Select(double[] values)
	{
		double totalValue = values.Sum();

		double[] probabilities = new double[values.Length];

		if (totalValue != 0)
		{
			for (int i = 0; i < values.Length; i++)
			{
				probabilities[i] = values[i] / totalValue;
			}
		}



		double p = r.NextDouble();
		double cumulative = 0;

		for (int i = 0; i < probabilities.Length; i++)
		{
			cumulative += probabilities[i];
			if (p < cumulative)
				return i;
		}
		return probabilities.Length - 1;
	}
}
