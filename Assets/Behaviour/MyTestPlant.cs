using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyTestPlant : MonoBehaviour, ISensable
{

    public Species specie = Species.Plant;

    public Species GetSpecies()
    {
        return specie;
    }



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
