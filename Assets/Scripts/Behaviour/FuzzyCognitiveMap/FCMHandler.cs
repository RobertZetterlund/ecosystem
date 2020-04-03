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

    public abstract void ProcessSensedObjects(Animal animal, SensedEvent sE);

    public abstract FCMHandler Reproduce(FCMHandler mateHandler);

    public void ProcessAnimal(double hunger, double thirst, double energy, double dietFactor, 
        bool isMale, int nChildren, double size, double speed, bool isFertile)
    {
        double fertility = isFertile ? 1 : 0;
        fcm.SetState(EntityField.Fertile, fertility);
        fcm.SetState(EntityField.NotFertile, 1 - fertility);

        fcm.SetState(EntityField.Hungry, hunger);
        fcm.SetState(EntityField.NotHungry, 1-hunger);

        fcm.SetState(EntityField.Thirsty, thirst);
        fcm.SetState(EntityField.NotThirsty, 1-thirst);
    }

    // Returns the translated weights
    public double[,] GetWeights()
    {
        return fcm.GetTranslatedWeights();
    }

    public StringBuilder ToCsv()
    {
        return ToCsv(GetWeights());
    }

    public static StringBuilder ToCsv(double[,] weights)
    {
        StringBuilder csv = new StringBuilder("");
        csv.Append("from,to,weight");
        csv.AppendLine();

        
        for (int _from = 0; _from < weights.GetLength(0); _from++)
        {
            for (int _to = 0; _to < weights.GetLength(1); _to++)
            {
                double weight = weights[_from, _to];
                csv.Append(((EntityField)_from).ToString() + "," + ((EntityField)_to).ToString() + "," + weight.ToString(System.Globalization.CultureInfo.InvariantCulture));
                csv.AppendLine();
            }      
        }

        return csv;
       
    }

}
