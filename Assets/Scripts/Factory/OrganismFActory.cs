using System;
using UnityEngine;
using UnityEngine.AI;
using Assets.Scripts;
/*
 * Class for creating animals
 */
public static class OrganismFactory
{
    public static void CreateAnimal(AnimalTraits traits, Vector3 location)
    {
        GameObject gameObject;
        switch (traits.species)
        {
            case Species.Rabbit:
                gameObject = CreateRabbit();
                break;
            default:
                gameObject = null;
                break;
        }

        Animal animal = gameObject.AddComponent<Animal>();
        animal.Init(traits);
        animal.transform.position = location;

        



        //UnityEngine.Object.Instantiate(child); // created clones so prolly dont need this
    }

    private static GameObject CreateRabbit()
    {
        //return GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        GameObject model = GameObject.Instantiate((GameObject)Resources.Load("testR"));
        //model.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        return model;
    }

    public static void CreatePlant(int size, Vector3 location)
    {
        //GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //GameObject model = (GameObject)Resources.Load("bush2");
        GameObject model = GameObject.Instantiate((GameObject)Resources.Load("Tree"));
        MyTestPlant plant = model.AddComponent<MyTestPlant>();
        plant.Init(size);
        plant.transform.position = location;
        //UnityEngine.Object.Instantiate(child); // created clones so prolly dont need this
    }
    // Get offset for the corresponding navmesh agent so it doesn't clip with the ground
    public static float GetOffset(Species species)
    {
        switch (species)
        {
            case Species.Rabbit:
                return 0f;
            case Species.Plant:
                return 0.1f;
            default:
                return 0f;
        }
    }
}
