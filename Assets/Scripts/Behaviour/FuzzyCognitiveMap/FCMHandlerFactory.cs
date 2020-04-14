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

	public static FoxFCMHandler CreateFoxFCMHandler(FCM fcm)
	{
		return new FoxFCMHandler(fcm);
	}


	public static FCMHandler getFCMHandlerSpecies(FCM fcm, Species species)
	{
		if (species == Species.Rabbit)
		{
			return CreateRabbitFCMHandler(fcm);
		}
		else if (species == Species.Fox)
		{
			return CreateFoxFCMHandler(fcm);
		}
		else
		{
			throw new Exception("Unable to find an FCMHandler for species " + species.ToString());
		}
	}

}


