using System;
using System.Collections;
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
        sE.GetWeightMap();


        // EntityInput som är surrounding, typ FoodPresence
        // 
       
        SetInverseDistanceInputFields(EntityInput.FoodClose, EntityInput.FoodFar, sE.GetFood(), animal);
        SetInverseDistanceInputFields(EntityInput.WaterClose, EntityInput.WaterFar, sE.GetWater(), animal);
        SetInverseDistanceInputFields(EntityInput.MateClose, EntityInput.MateFar, sE.GetMate(), animal);
        SetInverseDistanceInputFields(EntityInput.FoeClose, EntityInput.FoeFar, sE.GetFoe(), animal);
            
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
