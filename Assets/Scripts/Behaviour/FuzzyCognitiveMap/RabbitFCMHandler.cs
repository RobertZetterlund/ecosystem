using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RabbitFCMHandler : FCMHandler
{
	public RabbitFCMHandler(FCM fcm) : base(fcm)
	{
		fuzzifier = new DistanceFuzzifier();
	}

	// Sets the fcm values accordingly when something has been spotted


	public override void ProcessSensedObjects(Animal animal, SensedEvent sE)
	{
		Dictionary<string, float> fcmInput = CorrectWeightMapToInputs(sE.GetWeightMap());

		// presence
		SetInversePresenceInputFields(EntityInput.FoePresenceHigh, EntityInput.FoePresenceLow, fcmInput["Foe"]);
		SetInversePresenceInputFields(EntityInput.FoodPresenceHigh, EntityInput.FoodPresenceLow, fcmInput["Food"]);
		SetInversePresenceInputFields(EntityInput.WaterPresenceHigh, EntityInput.WaterPresenceLow, fcmInput["Water"]);
		SetInversePresenceInputFields(EntityInput.MatePresenceHigh, EntityInput.MatePresenceLow, fcmInput["Mate"]);

		// closest "most important"
		SetInverseDistanceInputFields(EntityInput.FoodClose, EntityInput.FoodFar, sE.GetFood(), animal);
		SetInverseDistanceInputFields(EntityInput.WaterClose, EntityInput.WaterFar, sE.GetWater(), animal);
		SetInverseDistanceInputFields(EntityInput.MateClose, EntityInput.MateFar, sE.GetMate(), animal);
		SetInverseDistanceInputFields(EntityInput.FoeClose, EntityInput.FoeFar, sE.GetFoe(), animal);

	}

	public override FCMHandler Reproduce(FCMHandler mateHandler)
	{
		return new RabbitFCMHandler(fcm.Reproduce(((RabbitFCMHandler)mateHandler).fcm));
	}
}
