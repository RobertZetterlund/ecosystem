using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
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

    /*public string GenerateJSON()
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
    }*/

    public string ToJson()
    {
        EntityInput[] inputs = (EntityInput[])Enum.GetValues(typeof(EntityInput));
        EntityAction[] actions = (EntityAction[])Enum.GetValues(typeof(EntityAction));
        EntityField[] fields = new EntityField[inputs.Length + actions.Length];
        Array.Copy(inputs, fields, inputs.Length);
        Array.Copy(actions, 0, fields, inputs.Length, actions.Length);

        string[] fieldsAsString = new string[fields.Length];

        for(int i = 0; i < fields.Length; i++)
        {
            fieldsAsString[i] = fields[i].ToString();
        }

        JsonObject jwa = new JsonObject();
        //jwa.fields = fieldsAsString;

        double[,] weights = fcm.GetWeights();
        double[,] convertedWeights = new double[fields.Length,fields.Length];

        TwoWayMap<int, int> translation = fcm.GetTranslation();

        for (int _from = 0; _from < weights.GetLength(0); _from++)
        {
            for (int _to = 0; _to < weights.GetLength(1); _to++)
            {
                double weight = weights[_from,_to];
                convertedWeights[translation.Reverse[_from], translation.Reverse[_to]] = weight;
            }
        }

        jwa.weights = convertedWeights;

        return JsonUtility.ToJson(jwa);
    }

    public StringBuilder ToCsv()
    {
        StringBuilder csv = new StringBuilder("");

        EntityField[] fields = (EntityField[])Enum.GetValues(typeof(EntityField));

        bool run = fields.Length > 0;
        int i = 0;
        csv.Append("field,");
        while (run)
        {
            csv.Append(fields[i].ToString());
            if(i++ +1 < fields.Length)
                csv.Append(",");
            else
                break;
        }
        return csv;
       
    }

    [Serializable]
    private class JsonObject
    {
        [SerializeField]
        public double[,] weights;
        //public string[] fields;
    }

    /*
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
    }*/

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
