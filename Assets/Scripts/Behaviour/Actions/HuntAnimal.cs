using System;

public class HuntAnimal : SearchAndAct
{
    public HuntAnimal(Animal animal) : base(animal)
    {

    }

    protected override bool Act()
    {
        if(!base.Act())
            return false;
        Animal prey = animal.targetGameObject.GetComponent<Animal>();
        prey.Kill();
        return true;
        
    }
}

