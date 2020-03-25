using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/**
 * FCMHandler works as an observer to GameObject so that senses can be passed to the handler
 * from a SenseRegistrator
 * 
 */
public abstract class FCMHandler
{
    protected FCM fcm;

    public FCMHandler(FCM fcm)
    {
        this.fcm = fcm;
    }

    public void CalculateFCM()
    {
        fcm.Calculate();
    }

    public virtual EntityAction GetAction()
    {
        return fcm.GetAction();
    }

    public string GetFCMData()
    {
        return fcm.ToString();
    }

    public FCM GetFCM()
    {
        return fcm;
    }

    public abstract void ProcessSensedObjects(Animal animal, ArrayList gameObjects);

    public abstract FCMHandler Reproduce(FCMHandler mateHandler);

    public void ProcessAnimal(double hunger, double thirst, double energy, double dietFactor, 
        bool isMale, int nChildren, double size, double speed, bool isFertile)
    {
        double fertility = isFertile ? 1 : 0;
        fcm.SetState(EntityField.Fertile, fertility);
        fcm.SetState(EntityField.NotFertile, 1 - fertility);

        fcm.SetState(EntityField.Hungry, hunger);
        fcm.SetState(EntityField.NotHungry, 1-hunger);
    }

    public string GenerateJSON()
    {

        double[,] weights = fcm.GetWeights();

        //JSONWeight[] jsonWeights = new JSONWeight[weights.GetLength(0) * weights.GetLength(1)];
        List<JSONWeight> jsonWeights = new List<JSONWeight>();

        TwoWayMap<int, int> translation = fcm.GetTranslation();

        for(int _from = 0, i = 0; _from < weights.GetLength(0); _from ++)
        {
            for (int _to = 0; _to < weights.GetLength(1); _to ++, i++)
            {
                double weight = weights[_from, _to];
                if(weight == 0)
                {
                    continue;
                }

                JSONWeight w = new JSONWeight();

                EntityField from = (EntityField)translation.Reverse[_from];
                EntityField to = (EntityField)translation.Reverse[_to];

                w.from = from.ToString();
                w.to = to.ToString();
                w.weight = weight;

                jsonWeights.Add(w);
            }
        }

        JSONWeightList jwl = new JSONWeightList();
        jwl.weights = jsonWeights.ToArray();

        return JsonUtility.ToJson(jwl);
    }

    [Serializable]
    private class JSONWeight
    {
        public string from;
        public string to;
        public double weight;
    }

    [Serializable]
    private class JSONWeightList
    {
        public JSONWeight[] weights;
    }

    public void SaveFCM(string json, string path)
    {
        string name = null;
#if UNITY_EDITOR
        name = path + ".json";
#endif
#if UNITY_STANDALONE
        // You cannot add a subfolder, at least it does not work for me
        name = path + ".json";
#endif

        string str = json;
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
}
