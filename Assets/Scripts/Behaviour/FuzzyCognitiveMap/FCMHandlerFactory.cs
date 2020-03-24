using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

  
public static class FCMHandlerFactory
{
    public static RabbitFCMHandler CreateRabbitFCMHandler(FCM fcm)
    {
        return new RabbitFCMHandler(fcm);
    }


    public static FCMHandler getFCMHandlerSpecies(FCM fcm, Species species)
    {
        if(species == Species.Rabbit)
        {
            return CreateRabbitFCMHandler(fcm);
        }
        else
        {
            return null;
        }
    }
       
}


