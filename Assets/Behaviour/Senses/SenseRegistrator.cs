using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SenseRegistrator
{
    private Animal animal;
    private FCM fcm; 
    private IFuzzifier fuzzifier;

    public SenseRegistrator(Animal animal)
    {
        this.animal = animal;
        fcm = animal.GetFCM();
        fuzzifier = new DistanceFuzzifier();
    }

    public void Register(GameObject sensedObject)
    {
        if(sensedObject.CompareTag("Plant"))
        {
            SetInverseDistanceInputFields(EntityInput.FoodClose, EntityInput.FoodFar, sensedObject);
        }

        if (sensedObject.CompareTag("Water"))
        {
            SetInverseDistanceInputFields(EntityInput.WaterClose, EntityInput.WaterFar, sensedObject);
        }
    }

    private void SetInverseDistanceInputFields(EntityInput close, EntityInput far, GameObject gameObject)
    {
        float dist = (gameObject.transform.position - animal.transform.position).magnitude;
        float standard = fuzzifier.Fuzzify(0, animal.GetSenseRadius(), dist);
        float inverse = 1 - standard;
        fcm.SetState((EntityField)close, standard);
        fcm.SetState((EntityField)far, inverse);
    }
}
