using System;
using UnityEngine;

/*
 * Class for creating animals
 */
public static class OrganismFactory
{
    public static void CreateAnimal(AnimalType type, double size, double dietFactor, int nChildren, Vector3 location)
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
    }

    private static GameObject CreateRabbit()
    {
        return GameObject.CreatePrimitive(PrimitiveType.Cylinder);
    }

    public static void CreatePlant(int size, Vector3 location)
    {
        GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        MyTestPlant plant = gameObject.AddComponent<MyTestPlant>();
        plant.Init(size);
        plant.transform.position = location;
        //UnityEngine.Object.Instantiate(child); // created clones so prolly dont need this
    }
}
