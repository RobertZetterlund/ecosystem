using System;


class GaussianMutation : SingletonBase<GaussianMutation>, IMutation
{

	//private static double STD_DEVIATION_FACTOR = 0.5;
	//private static double MUTATION_CHANCE = 0.05;
	private Random r = new Random();

	public RangedDouble Mutate(RangedDouble gene)
	{
		if (r.NextDouble() >= SimulationController.Instance().settings.mutation_chance)
		{
			return gene.Duplicate();
		}
		double value = gene.GetValue();
		double lower = gene.GetLower();
		double upper = gene.GetUpper();
		double deviation = 0.5*SimulationController.Instance().settings.std_dev; ;
		double mutation = MathUtility.RandomGaussian(value, deviation);


		mutation = MathUtility.Clamp(mutation, lower, upper);
		return new RangedDouble(mutation, lower, upper);
	}
}