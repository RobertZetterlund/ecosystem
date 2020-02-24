using System;
using UnityEngine;

class RabbitSenseRegistrator : SenseRegistrator
{

    public RabbitSenseRegistrator(FCM fcm) : base(fcm)
    {

    }

    public override void Register(GameObject sensedObject)
    {
        Species specie;
        if (Enum.TryParse(sensedObject.tag, out specie))
        {
            switch (specie)
            {
                case Species.Plant:
                    fcm.ImpactState(EntityField.FoodClose, 1);
                    break;
            }
        }
            
    }
}
