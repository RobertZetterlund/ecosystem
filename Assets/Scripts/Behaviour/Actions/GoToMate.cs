using System;
using UnityEngine;

public class GoToMate : SearchAndAct
{
    public GoToMate(Animal animal) : base(animal)
    {
        
    }

    protected override bool Act()
    {
        if (!base.Act())
            return false;

        // if mate wasnt fertile, search for new
        try
        {
            Animal mate = animal.targetGameObject.GetComponent<Animal>();
            // if mate wasnt fertile, search for new
            return animal.Reproduce(mate);
            
        }
        catch (MissingReferenceException)
        {
            // if mate died, search for new
        }
        Reset();
        return false;
    }
}
