using System;
using UnityEngine;

public static class AnimalFactory
{
    public static Animal CreateAnimal(AnimalType type)
    {
        switch (type)
        {
            case AnimalType.Rabbit:
                return CreateRabbit();
            default:
                return null;
        }
    }

    private static Animal CreateRabbit()
    {
        GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        Animal rabbit = gameObject.AddComponent<Animal>();
        rabbit.SetType(AnimalType.Rabbit);
        return rabbit;
    }
}
