using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class TraitLogger : MonoBehaviour
{
    private static (double,string)[][] currentTotals = new (double, string)[Species.GetValues(typeof(Species)).Length][];
    private static int[] nAnimals = new int[Species.GetValues(typeof(Species)).Length];
    // current total might not exist when all animals are dead, but we still want to log so we need this
    private static int[] loggableSpecies = new int[Species.GetValues(typeof(Species)).Length];
    private int counter = 2; // dont log at t=0
    public static bool logNext = false;
    private int logInterval = 100;
    private bool firstSave = true;
    private string filename; 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        counter++;
        if (counter % logInterval == 0)
        {
            logNext = true;
        } else
        {
            logNext = false;
        }
    }

    void LateUpdate()
    {
        if (counter % logInterval == 1) // need to wait 1 extra update so animals can log themselves
        {
            // maybe need to add empty entries for when a species is temporarily dead

            Save();
            for (int i = 0; i < Species.GetValues(typeof(Species)).Length; i++)
            {
                nAnimals[i] = 0; // refresh 
            }
        }
    }

    public static void Log(AnimalTraits traits)
    {
        nAnimals[(int)traits.species]++;
        loggableSpecies[(int)traits.species] = 1;
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
    private void Save()
    {
        StringBuilder row = MakeRow(false);
        if (firstSave)
        {
            row = MakeRow(true).Append("\n").Append(row);
            filename = "Logs/Trait Log " + DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss");
            firstSave = false;
        }
        //File.WriteAllText(filename, row.ToString());
        using (StreamWriter writeText = new StreamWriter(filename +".txt", true))
        {
            writeText.WriteLine(row.ToString());
        }
    }

    private StringBuilder MakeRow(bool isHeader)
    {
        StringBuilder row = new StringBuilder("");
        for (int i = 0; i < loggableSpecies.Length; i++)
        {
            // make 1 column or each trait and species
            if (loggableSpecies[i] == 1)
            {
                // make entry for each population
                if (isHeader)
                {
                    row.Append(((Species)i).ToString());
                    row.Append('-');
                    row.Append("population, ");
                } else
                {
                    row.Append(nAnimals[i]);
                    row.Append(", ");
                }
                // make an entry for each trait
                for (int j = 0; j < currentTotals[i].Length; j++)
                {
                    if (isHeader)
                    {
                        // "e.g Rabbit-speed"
                        row.Append(((Species)i).ToString());
                        row.Append('-');
                        row.Append(currentTotals[i][j].Item2);
                    } else
                    {
                        row.Append(currentTotals[i][j].Item1/nAnimals[i]); // average
                    }
                    row.Append(", ");
                }
            }
        }
        row.Length -= 2; // remove ", "
        return row;
    }
}
