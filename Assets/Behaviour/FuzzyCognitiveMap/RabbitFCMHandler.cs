using System;
using UnityEngine;

class RabbitFCMHandler : FCMHandler
{
    private IFuzzifier fuzzifier;
    public RabbitFCMHandler(Animal animal) : base(animal)
    {
        fcm = FCMFactory.RabbitFCM();
        fuzzifier = new DistanceFuzzifier();
    }

    // Sets the fcm values accordingly when something has been spotted
    public override void OnNext(GameObject value)
    {
        if(value.CompareTag("Plant"))
        {
            SetInverseDistanceInputFields(EntityInput.FoodClose, EntityInput.FoodFar, value);
        }

        if (value.CompareTag("Water"))
        {
            SetInverseDistanceInputFields(EntityInput.WaterClose, EntityInput.WaterFar, value);
        }
    }

    // Sets the two input fields as "komplement" to eachother in the fcm.
    private void SetInverseDistanceInputFields(EntityInput close, EntityInput far, GameObject gameObject)
    {
        float dist = (gameObject.transform.position - animal.transform.position).magnitude;
        float standard = fuzzifier.Fuzzify(0, animal.GetSenseRadius(), dist);
        float inverse = 1 - standard;
        fcm.SetState((EntityField)close, standard);
        fcm.SetState((EntityField)far, inverse);
    }
}
