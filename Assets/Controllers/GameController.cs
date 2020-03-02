using System;
using UnityEngine;

public class GameController : MonoBehaviour
{

    void Start()
    {
        // spawn first rabbit
        OrganismFactory.CreateAnimal(AnimalType.Rabbit, 1, 1, 3, new Vector3(5.83f, 1f, 14.36f));
        // spawn first plant
        OrganismFactory.CreatePlant(1, new Vector3(5.83f, 0.5f, 10.36f));
    }
}
