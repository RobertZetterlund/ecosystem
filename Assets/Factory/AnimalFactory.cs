using System;
using UnityEngine;

/*
 * Class for creating animals
 */
public static class AnimalFactory
{
    public static Animal CreateAnimal(AnimalType type, double size, double dietFactor, int nChildren, Vector3 location)
    {
        GameObject gameObject;
        switch (type)
        {
            case AnimalType.Rabbit:
                gameObject = CreateRabbit();
                break;
            default:
                gameObject = null;
                break;
        }

        Animal animal = gameObject.AddComponent<Animal>();
        animal.Init(type, size, dietFactor, nChildren);
        animal.transform.position = location;
        //UnityEngine.Object.Instantiate(child); // created clones so prolly dont need this
        return animal;
    }

    private static GameObject CreateRabbit()
    {
        return GameObject.CreatePrimitive(PrimitiveType.Cylinder);
    }
}
