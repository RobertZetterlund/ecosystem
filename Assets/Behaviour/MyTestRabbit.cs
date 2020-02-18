using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyTestRabbit : MonoBehaviour, Consumer
{

    double Hunger = 1;
    bool isMale = true;
    double speed = 5;

    // Start is called before the first frame update
    void Start()
    {
        // use Köres senses to locate a food source
        // go to food source
        // else
        // walk and loiter about
    }

    // Update is called once per frame
    void Update()
    {
        // look for enemy
        transform.Translate(0.000000001f * Time.deltaTime, 0f, 0f);


    }
}
