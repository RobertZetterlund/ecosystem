using System;

class NoMutation : SingletonBase<NoMutation>, IMutation
{
	public RangedDouble Mutate(RangedDouble gene)
	{
		return gene.Duplicate();
	}
}

