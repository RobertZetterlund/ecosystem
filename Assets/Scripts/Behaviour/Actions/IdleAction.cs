using System;

public class IdleAction : AbstractAction
{
    public IdleAction(Animal animal) : base(animal)
    {
    }

    public override void Execute()
    {
        
    }

    public override bool IsDone()
    {
        throw new NotImplementedException();
    }

    public override void Reset()
    {
        
    }
}

