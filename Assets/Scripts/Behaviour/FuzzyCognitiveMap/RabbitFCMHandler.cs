using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RabbitFCMHandler : FCMHandler
{
    private IFuzzifier fuzzifier;
    public RabbitFCMHandler(FCM fcm) : base(fcm)
    {
        fuzzifier = new DistanceFuzzifier();
    }

    // Sets the fcm values accordingly when something has been spotted


    public override void ProcessSensedObjects(Animal animal, SensedEvent sE)
    {
        Dictionary<string,int> fcmInput = CorrectWeightMapToInputs(sE.GetWeightMap());


        // EntityInput som är surrounding, typ FoodPresence

        /*foreach(KeyValuePair<string, int> item in fcmInput)
        {
            float value = calculatePresenceInput(item.Value);

            if (item.Key.Equals("Food")) {
                SetInversePresenceInputFields(EntityInput.FoodPresenceHigh, EntityInput.FoodPresenceLow, value);
            }
            else if(item.Key.Equals("Mate"))
            {
                SetInversePresenceInputFields(EntityInput.MatePresenceHigh, EntityInput.MatePresenceLow, value);
            }
            else if(item.Key.Equals("Foe"))
            {
                SetInversePresenceInputFields(EntityInput.FoePresenceHigh, EntityInput.FoePresenceLow, value);
            }
            else if(item.Key.Equals("Water"))
            {
                SetInversePresenceInputFields(EntityInput.WaterPresenceHigh, EntityInput.WaterPresenceLow, value);
            }
        }*/
        
        SetInverseDistanceInputFields(EntityInput.FoodClose, EntityInput.FoodFar, sE.GetFood(), animal);
        SetInverseDistanceInputFields(EntityInput.WaterClose, EntityInput.WaterFar, sE.GetWater(), animal);
        SetInverseDistanceInputFields(EntityInput.MateClose, EntityInput.MateFar, sE.GetMate(), animal);
        SetInverseDistanceInputFields(EntityInput.FoeClose, EntityInput.FoeFar, sE.GetFoe(), animal);
            
    }

    private float calculatePresenceInput(int count)
    {
        float weight = (float)Math.Log(count - 1);
        return weight > 1 ? 1 : weight;
    }

    private void SetInversePresenceInputFields(EntityInput close, EntityInput far, float value)
    {
        fcm.SetState((EntityField)close, value);
        fcm.SetState((EntityField)far, 1- value);
    }


    private Dictionary<string,int> CorrectWeightMapToInputs(Dictionary<string,int> weightMap)
    {
        



        return weightMap;
    }

    // Sets the two input fields as "komplement" to eachother in the fcm.
    private void SetInverseDistanceInputFields(EntityInput close, EntityInput far, GameObject sensedObject, Animal animal)
    {
        float standard = 0;
        float inverse = 1;
        if(sensedObject != null)
        {
            float dist = (sensedObject.transform.position - animal.transform.position).magnitude;
            standard = fuzzifier.Fuzzify(0, 100, dist);
            inverse = 1 - standard;
        }
        fcm.SetState((EntityField)close, standard);
        fcm.SetState((EntityField)far, inverse);
    }

    public override FCMHandler Reproduce(FCMHandler mateHandler)
    {
        return new RabbitFCMHandler(fcm.Reproduce(((RabbitFCMHandler)mateHandler).fcm));
    }
}
