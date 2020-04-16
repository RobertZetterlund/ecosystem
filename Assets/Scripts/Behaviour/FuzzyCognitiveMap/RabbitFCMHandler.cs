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

	// Fuzzifier for presence
	private float calculatePresenceInput(int count)
	{
		float weight = (float)Math.Log10(count + 1);
		return weight > 1 ? 1 : weight;
	}

	private void SetInversePresenceInputFields(EntityInput close, EntityInput far, float value)
	{
		fcm.SetState((EntityField)close, value);
		fcm.SetState((EntityField)far, 1 - value);
	}


	private Dictionary<string, float> CorrectWeightMapToInputs(Dictionary<string, int> weightMap)
	{

		Dictionary<string, int>.Enumerator enumerator = weightMap.GetEnumerator();
		Dictionary<string, float> fcmInput = new Dictionary<string, float>();

		while (enumerator.MoveNext())
		{
			KeyValuePair<string, int> curr = enumerator.Current;
			float adjustedvalue = calculatePresenceInput(curr.Value);
			fcmInput.Add(curr.Key, adjustedvalue);
		}

		return fcmInput;
	}

	// Sets the two input fields as "komplement" to eachother in the fcm.
	private void SetInverseDistanceInputFields(EntityInput close, EntityInput far, GameObject sensedObject, Animal animal)
	{
		float standard = 0;
		float inverse = 1;
		if (sensedObject != null)
		{
			float dist = Utility.DistanceBetweenTwoGameObjects(sensedObject, animal.gameObject);
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
