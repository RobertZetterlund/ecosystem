using System;


class ConstantMutation : SingletonBase<ConstantMutation>, IMutation
{

	//private static double STD_DEVIATION_FACTOR = 0.5;
	//private static double MUTATION_CHANCE = 0.05;
	private Random r = new Random();
	private double mutationSize = 0.15;

	public RangedDouble Mutate(RangedDouble gene)
	{
		if (r.NextDouble() >= SimulationController.Instance().settings.mutation_chance)
		{
			return gene.Duplicate();
		}
		double mutation = mutationSize * r.NextDouble()*((r.NextDouble() < 0.5 ? -1 : 1));
		RangedDouble geneB = gene.Duplicate();
		geneB.Add(mutation);
		return geneB;
	}
}