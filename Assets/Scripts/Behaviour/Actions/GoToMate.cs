using System;
using UnityEngine;

public class GoToMate : AbstractAction
{
    private SearchAction search;
    public GoToMate(Animal animal) : base(animal)
    {
        search = new SearchAction(animal);
    }

    public override void Execute()
    {
        switch (state)
        {
            case ActionState.Mating:
                Mate();
                break;
            default:
                Search();
                break;
        }
    }

    private void Search()
    {
        search.Execute();
        if (search.IsDone())
            Mate();
    }

    private void Mate()
    {
        state = ActionState.Mating;

        // if mate wasnt fertile, search for new
        try
        {
            Animal mate = animal.targetGameObject.GetComponent<Animal>();
            // if mate wasnt fertile, search for new
            animal.Reproduce(mate);
            
        }
        catch (MissingReferenceException)
        {
            // if mate died, search for new
        }
        Reset();
    }

    public override bool IsDone()
    {
        throw new NotImplementedException();
    }

    public override void Reset()
    {
        state = ActionState.Searching;
    }
}
