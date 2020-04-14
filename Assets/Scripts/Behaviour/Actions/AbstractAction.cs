using System;

public abstract class AbstractAction
{
	protected Animal animal;
	protected ActionState state;


	public AbstractAction(Animal animal)
	{
		this.animal = animal;
	}
	public abstract void Execute();
	public abstract void Reset();
	public abstract bool IsDone();
	public ActionState GetState()
	{
		return state;
	}
}

