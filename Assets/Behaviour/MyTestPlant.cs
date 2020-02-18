using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyTestPlant : MonoBehaviour
{

    public Species specie = Species.Plant;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.tag = "Plant";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
