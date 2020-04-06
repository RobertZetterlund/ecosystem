using System;
using UnityEngine;
using UnityEngine.AI;
/*
 * Class for creating animals
 */
public static class OrganismFactory
{
    public static void CreateAnimal(AnimalTraits traits, Vector3 location)
    {
        Animal animal;
        switch (traits.species)
        {
            case Species.Rabbit:
                animal = CreateRabbit();
                break;
            case Species.Fox:
                animal = CreateFox();
                break;
            default:
                animal = null;
                break;
        }

        animal.Init(traits);
        animal.transform.position = location;

        



        //UnityEngine.Object.Instantiate(child); // created clones so prolly dont need this
    }

    private static Animal CreateFox()
    {
        //return GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        GameObject model = GameObject.Instantiate((GameObject)Resources.Load("testF")); //name a Fox to testF in unity
        Animal animal = model.AddComponent<Fox>();
        //model.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        return animal;
    }

    private static Animal CreateRabbit()
    {
        //return GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        GameObject model = GameObject.Instantiate((GameObject)Resources.Load("testR"));
        Animal animal = model.AddComponent<Rabbit>();
        //model.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        return animal;
    }

    public static void CreatePlant(int size, Vector3 location)
    {
        //GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //GameObject model = (GameObject)Resources.Load("bush2");
        GameObject model = GameObject.Instantiate((GameObject)Resources.Load("Tree"));
        Plant plant = model.AddComponent<Plant>();
        plant.Init(size);
        plant.transform.position = location;
        //UnityEngine.Object.Instantiate(child); // created clones so prolly dont need this
    }

    public static void CreateSappling(int size, Vector3 location)
    {
        GameObject model = GameObject.Instantiate((GameObject)Resources.Load("Sappling"));
        Sappling sappling = model.AddComponent<Sappling>();
        sappling.Init(size);
        sappling.transform.position = location;
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

    public static Vector3 GetOriginalScale(Species species)
    {
        switch (species)
        {
            case Species.Rabbit:
                return new Vector3(1f, 1f, 1f);
            case Species.Plant:
                return new Vector3 (1f, 1f, 1f);
            default:
                return new Vector3(1f, 1f, 1f);
        }
    }
}
