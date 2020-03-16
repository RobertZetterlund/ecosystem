using System;
using System.Collections;
using UnityEngine;

class RabbitFCMHandler : FCMHandler
{
    private IFuzzifier fuzzifier;
    public RabbitFCMHandler(FCM fcm) : base(fcm)
    {
        fuzzifier = new DistanceFuzzifier();
    }

    // Sets the fcm values accordingly when something has been spotted
    public override void ProcessSensedObjects(Animal animal, ArrayList gameObjects)
    {
        foreach(GameObject value in gameObjects)
        {
            if(value.CompareTag("Plant"))
            {
                SetInverseDistanceInputFields(EntityInput.FoodClose, EntityInput.FoodFar, value, animal);
            }

            if (value.CompareTag("Water"))
            {
                SetInverseDistanceInputFields(EntityInput.WaterClose, EntityInput.WaterFar, value, animal);
            }
        }
            
    }

    // Sets the two input fields as "komplement" to eachother in the fcm.
    private void SetInverseDistanceInputFields(EntityInput close, EntityInput far, GameObject sensedObject, Animal animal)
    {
        float dist = (sensedObject.transform.position - animal.transform.position).magnitude;
        float standard = fuzzifier.Fuzzify(0, animal.GetSenseRadius(), dist);
        float inverse = 1 - standard;
        fcm.SetState((EntityField)close, standard);
        fcm.SetState((EntityField)far, inverse);
    }

    public override FCMHandler Reproduce(FCMHandler mateHandler)
    {
        return new RabbitFCMHandler(fcm.Reproduce(((RabbitFCMHandler)mateHandler).fcm));
    }
}
