using System;
using UnityEngine;

public class GameController : MonoBehaviour
{

    void Start()
    {
        // spawn first rabbit
        AnimalFactory.CreateAnimal(AnimalType.Rabbit, 1, 1, 3, new Vector3(5.83f, 1f, 14.36f));
    }
}
