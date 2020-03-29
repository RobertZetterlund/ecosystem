using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class TraitLogger : MonoBehaviour
{
    [SerializeField]
    public bool enable = false; // set true to log
    
    private static (double,string)[][] currentTraitTotals = new (double, string)[Species.GetValues(typeof(Species)).Length][];
    private static List<FCMHandler>[] recentFCMs = new List<FCMHandler>[Species.GetValues(typeof(Species)).Length];
    private static int[] nAnimals = new int[Species.GetValues(typeof(Species)).Length];
    // current total might not exist when all animals are dead, but we still want to log so we need this
    private static int[] loggableSpecies = new int[Species.GetValues(typeof(Species)).Length];
    private int counter = 2; // dont log at t=0
    public static bool logNext = false;
    private int logInterval = 100;
    private bool firstSave = true;
    private string folder; 

    // Start is called before the first frame update
    void Start()
    {
        if (enable)
        {
            folder = "Python Scripts and Logs/Logs/" + DateTime.Now.ToString("M-dd--HH-mm-ss");
            Directory.CreateDirectory(folder);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!enable)
        {
            return;
        }
        counter++;
        if (counter % logInterval == 0)
        {
            logNext = true;
            for (int i = 0; i < Species.GetValues(typeof(Species)).Length; i++)
            {
                nAnimals[i] = 0; // refresh 
            }
        } else
        {
            logNext = false;
        }
    }

    void LateUpdate()
    {
        if (!enable)
        {
            return;
        }
        if (counter % logInterval == 1) // need to wait 1 extra update so animals can log themselves
        {
            // maybe need to add empty entries for when a species is temporarily dead
            Save();
        }
    }

    void OnApplicationQuit()
    {
        if (enable)
        {
            // save fcm
            for (int i = 0; i < recentFCMs.Length; i++)
            {
                if (recentFCMs[i] != null)
                {
                    StringBuilder s = FCMHandler.ToCsv(GenerateAverageFCM((Species)i));
                    SaveFCM(s.ToString(), folder + '/' + ((Species)i).ToString() + "_fcm");
                }
            }
        }
    }

    // Generate an average fcm of a species
    public double[,] GenerateAverageFCM(Species s)
    {
        int nFields = Enum.GetNames(typeof(EntityField)).Length;
        double[,] averageWeights = new double[nFields,nFields];
        foreach (FCMHandler fcm in recentFCMs[(int)s])
        {
            double[,] weights = fcm.GetWeights();
            for (int _from = 0; _from < weights.GetLength(0); _from++)
            {
                for (int _to = 0; _to < weights.GetLength(1); _to++)
                {
                    averageWeights[_from, _to] = weights[_from, _to] / nAnimals[(int)s];
                }
            }
        }
        return averageWeights;
    }

    public void SaveFCM(string content, string path)
    {
        string name = path + ".txt";
#if UNITY_EDITOR
        //name = path + ".txt";
#endif
#if UNITY_STANDALONE
        // You cannot add a subfolder, at least it does not work for me
        //name = path + ".txt";
#endif

        string str = content;
        using (FileStream fs = new FileStream(name, FileMode.Create))
        {
            using (StreamWriter writer = new StreamWriter(fs))
            {
                writer.Write(str);
            }
        }
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
    }

    public static void Log(AnimalTraits traits)
    {
        nAnimals[(int)traits.species]++;
        loggableSpecies[(int)traits.species] = 1;
        (double, string)[] traitValues = traits.GetNumericalTraits();

        if (nAnimals[(int)traits.species] == 1)
        {
            // refresh list
            currentTraitTotals[(int)traits.species] = traitValues;
            recentFCMs[(int)traits.species] = new List<FCMHandler>() { traits.fcmHandler };
        } else
        {
            for (int i = 0; i < traitValues.Length; i++)
            {
                currentTraitTotals[(int)traits.species][i].Item1 += traitValues[i].Item1;
                recentFCMs[(int)traits.species].Add(traits.fcmHandler);
            }
        }
        
    }

    // draw and or log current averages;
    private void Save()
    {
        StringBuilder row = MakeRow(false);
        // save traits
        if (firstSave)
        {
            row = MakeRow(true).Append("\n").Append(row);
            firstSave = false;
        }
        //File.WriteAllText(filename, row.ToString());
        using (StreamWriter writeText = new StreamWriter(folder + '/' + "Traits" + ".txt", true))
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
                    row.Append("population,");
                } else
                {
                    row.Append(nAnimals[i]);
                    row.Append(",");
                }
                // make an entry for each trait
                for (int j = 0; j < currentTraitTotals[i].Length; j++)
                {
                    if (isHeader)
                    {
                        // "e.g Rabbit-speed"
                        row.Append(((Species)i).ToString());
                        row.Append('-');
                        row.Append(currentTraitTotals[i][j].Item2);
                    } else
                    {
                        // average
                        row.Append((currentTraitTotals[i][j].Item1/nAnimals[i]).ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)); 
                    }
                    row.Append(",");
                }
            }
        }
        row.Length -= 1; // remove ","
        return row;
    }
}
