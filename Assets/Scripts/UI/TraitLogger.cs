using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraitLogger : MonoBehaviour
{
    private static (double,string)[][] currentTotals = new (double, string)[Species.GetValues(typeof(Species)).Length][];
    private static int[] nAnimals = new int[Species.GetValues(typeof(Species)).Length];

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LateUpdate()
    {
        for (int i = 0; i < Species.GetValues(typeof(Species)).Length; i++)
        {
            Visualize((Species)i);
            nAnimals[i] = 0; // refresh 

        }
    }

    public static void Log(AnimalTraits traits)
    {
        nAnimals[(int)traits.species]++;
        (double, string)[] traitValues = traits.GetNumericalTraits();

        if (nAnimals[(int)traits.species] == 1)
        {
            // refresh list
            currentTotals[(int)traits.species] = traitValues;
        } else
        {
            for (int i = 0; i < traitValues.Length; i++)
            {
                currentTotals[(int)traits.species][i].Item1 += traitValues[i].Item1;
            }
        }
    }

    // draw and or log current averages;
    private void Visualize(Species species)
    {

    }
}
