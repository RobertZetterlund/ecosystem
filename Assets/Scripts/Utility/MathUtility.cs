﻿using System;

public static class MathUtility
{
	public static Random random = new Random();

	public static double RandomGaussian(double mean, double stdDeviation)
	{
		double v1 = 1.0 - random.NextDouble(); //uniform(0,1] random doubles
		double v2 = 1.0 - random.NextDouble();
		double randStdNormal = Math.Sqrt(-2.0 * Math.Log(v1)) * Math.Sin(2.0 * Math.PI * v2); //random normal(0,1)
		double randNormal = mean + stdDeviation * randStdNormal; //random normal(mean,stdDev^2)
		return randNormal;
	}

	public static double RandomUniform(double lower, double upper)
	{
		return lower + (upper - lower) * random.NextDouble();
	}

	public static double Clamp(double value, double lower, double upper)
	{
		if (value > upper)
			return upper;
		else if (value < lower)
			return lower;
		return value;
	}

	public static bool RandomChance(double odds)
	{
		return random.NextDouble() <= odds;
	}
}
